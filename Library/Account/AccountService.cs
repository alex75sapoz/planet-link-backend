using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Account
{
    class AccountService : BaseService<AccountConfiguration, AccountRepository>, IAccountService
    {
        public AccountService(AccountConfiguration configuration, AccountRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache)
        {
            _googleAuthenticationApi = new RestClient(_configuration.GoogleApi.AuthenticationServer);
            _googleTokenApi = new RestClient(_configuration.GoogleApi.TokenServer);
            _stocktwitsApi = new RestClient(_configuration.StocktwitsApi.Server);
            _fitbitAuthenticationApi = new RestClient(_configuration.FitbitApi.AuthenticationServer);
            _fitbitTokenApi = new RestClient(_configuration.FitbitApi.TokenServer);
        }

        private readonly IRestClient _googleAuthenticationApi;
        private readonly IRestClient _googleTokenApi;
        private readonly IRestClient _stocktwitsApi;
        private readonly IRestClient _fitbitAuthenticationApi;
        private readonly IRestClient _fitbitTokenApi;

        #region Memory Cache

        internal static ConcurrentDictionary<int, AccountUserTypeContract> _userTypes = new();
        internal static ConcurrentDictionary<int, AccountUserGenderContract> _userGenders = new();
        internal static ConcurrentDictionary<int, AccountUserContract> _users = new();
        internal static ConcurrentDictionary<int, AccountUserSessionContract> _userSessions = new();

        public async Task MemoryCacheRefreshAsync(AccountDictionary? dictionary = null, int? id = null)
        {
            if (!dictionary.HasValue || dictionary.Value == AccountDictionary.UserTypes)
            {
                if (!id.HasValue)
                    _userTypes = new((await _repository.GetUserTypesAsync()).Select(userTypeEntity => userTypeEntity.MapToUserTypeContract()).ToDictionary(userType => userType.UserTypeId));
                else
                    _userTypes[id.Value] = (await _repository.GetUserTypeAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToUserTypeContract();
            }

            if (!dictionary.HasValue || dictionary.Value == AccountDictionary.UserGenders)
            {
                if (!id.HasValue)
                    _userGenders = new((await _repository.GetUserGendersAsync()).Select(userGenderEntity => userGenderEntity.MapToUserGenderContract()).ToDictionary(userGender => userGender.UserGenderId));
                else
                    _userGenders[id.Value] = (await _repository.GetUserGenderAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToUserGenderContract();
            }

            if (!dictionary.HasValue || dictionary.Value == AccountDictionary.Users)
            {
                if (!id.HasValue)
                    _users = new((await _repository.GetUsersAsync()).Select(userEntity => userEntity.MapToUserContract()).ToDictionary(user => user.UserId));
                else
                    _users[id.Value] = (await _repository.GetUserAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToUserContract();
            }

            if (!dictionary.HasValue || dictionary.Value == AccountDictionary.UserSessions)
            {
                if (!id.HasValue)
                    _userSessions = new((await _repository.GetUserSessionsAsync()).Select(userSessionEntity => userSessionEntity.MapToUserSessionContract()).ToDictionary(userSession => userSession.UserSessionId));
                else
                    _userSessions[id.Value] = (await _repository.GetUserSessionAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToUserSessionContract();
            }
        }

        #endregion

        #region Search

        public List<AccountUserContract> SearchUsers(string keyword, int userTypeId)
        {
            var userType = IAccountMemoryCache.GetUserType(userTypeId);

            return (userType.UserTypeId switch
            {
                (int)UserType.Google => IAccountMemoryCache.Users.Where(user =>
                    user.Value.UserTypeId == userType.UserTypeId &&
                    user.Value.Google!.Username.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                ),
                (int)UserType.Stocktwits => IAccountMemoryCache.Users.Where(user =>
                    user.Value.UserTypeId == userType.UserTypeId &&
                    user.Value.Stocktwits!.Username.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                ),
                (int)UserType.Fitbit => IAccountMemoryCache.Users.Where(user =>
                    user.Value.UserTypeId == userType.UserTypeId &&
                    user.Value.Fitbit!.FullName.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                ),
                _ => throw new BadRequestException($"{nameof(userTypeId)} is invalid")
            }).Take(_configuration.Limit.SearchUsersLimit)
              .Select(user => user.Value)
              .ToList();
        }

        #endregion

        #region Get

        public string GetUserConsentUrl(int userTypeId, string subdomain, string page)
        {
            var userType = IAccountMemoryCache.GetUserType(userTypeId);

            return userType.UserTypeId switch
            {
                (int)UserType.Google => GetUserGoogleConsentUrlResponse(subdomain, page),
                (int)UserType.Stocktwits => GetUserStocktwitsConsentUrlResponse(subdomain, page),
                (int)UserType.Fitbit => GetUserFitbitConsentUrlResponse(subdomain, page),
                _ => throw new BadRequestException($"{nameof(userTypeId)} is not supported")
            };
        }

        #endregion

        #region Authenticate

        public async Task<AccountUserSessionContract> AuthenticateUserTokenAsync(int userTypeId, string token, DateTimeZone timezone)
        {
            var userSession = IAccountMemoryCache.GetUserSession(userTypeId, token, isExpiredSessionValid: true);

            if (userTypeId == (int)UserType.Google)
            {
                if (!userSession.IsExpired && !userSession.IsAboutToExpire)
                    return userSession;

                //Update database
                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId) ?? throw new BadRequestException($"{nameof(userSession.UserSessionId)} is invalid");
                var userEntity = userSessionEntity.User;
                var googleEntity = userEntity.Google!;

                var googleRefreshTokenResponse = await GetUserGoogleRefreshTokenResponseAsync(userSessionEntity.RefreshToken);
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                userSessionEntity.RefreshToken = googleRefreshTokenResponse.RefreshToken ?? userSessionEntity.RefreshToken;
                userSessionEntity.Token = googleRefreshTokenResponse.Token;
                userSessionEntity.TokenExpiresOn = currentTime.AddSeconds(googleRefreshTokenResponse.TokenDurationInSeconds);
                userSessionEntity.LastUpdatedOn = currentTime;

                if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.GoogleUpdateThresholdInHours)
                {
                    var googleJsonWebToken = new JwtSecurityTokenHandler().ReadJwtToken(googleRefreshTokenResponse.UserJsonWebToken);

                    userEntity.LastUpdatedOn = currentTime;

                    if (googleJsonWebToken.Payload.TryGetValue("name", out object? name))
                        googleEntity.Name = name.ToString()!;

                    if (googleJsonWebToken.Payload.TryGetValue("email", out object? email))
                        googleEntity.Email = email.ToString()!;
                }

                await _repository.SaveChangesAsync();

                //Update memory cache
                var user = IAccountMemoryCache.GetUser(userEntity.UserId);
                var google = user.Google!;

                userSession.Token = userSessionEntity.Token;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                google.Name = googleEntity.Name;
                google.Username = googleEntity.Email;

                return userSession;
            }

            if (userTypeId == (int)UserType.Stocktwits)
            {
                if (!userSession.IsExpired && !userSession.IsAboutToExpire)
                    return userSession;

                //Update database
                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId) ?? throw new BadRequestException($"{nameof(userSession.UserSessionId)} is invalid");
                var userEntity = userSessionEntity.User;
                var stocktwitsEntity = userEntity.Stocktwits!;

                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                userSessionEntity.TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours);
                userSessionEntity.LastUpdatedOn = currentTime;

                if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.StocktwitsUpdateThresholdInHours)
                {
                    var stocktwitsResponse = await GetUserStocktwitsResponseAsync(userSessionEntity.Token);

                    userEntity.LastUpdatedOn = currentTime;

                    stocktwitsEntity.Name = stocktwitsResponse.Name;
                    stocktwitsEntity.Username = stocktwitsResponse.Username;
                    stocktwitsEntity.FollowersCount = stocktwitsResponse.FollowersCount;
                    stocktwitsEntity.FollowingsCount = stocktwitsResponse.FollowingsCount;
                    stocktwitsEntity.PostsCount = stocktwitsResponse.PostsCount;
                    stocktwitsEntity.LikesCount = stocktwitsResponse.LikesCount;
                    stocktwitsEntity.WatchlistQuotesCount = stocktwitsResponse.WatchlistQuotesCount;
                    stocktwitsEntity.CreatedOn = stocktwitsResponse.CreatedOn;
                }

                await _repository.SaveChangesAsync();

                //Update memory cache
                var user = IAccountMemoryCache.GetUser(userEntity.UserId);
                var stocktwits = user.Stocktwits!;

                userSession.Token = userSessionEntity.Token;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                stocktwits.Name = stocktwitsEntity.Name;
                stocktwits.Username = stocktwitsEntity.Username;
                stocktwits.FollowersCount = stocktwitsEntity.FollowersCount;
                stocktwits.FollowingsCount = stocktwitsEntity.FollowingsCount;
                stocktwits.PostsCount = stocktwitsEntity.PostsCount;
                stocktwits.LikesCount = stocktwitsEntity.LikesCount;
                stocktwits.WatchlistQuotesCount = stocktwitsEntity.WatchlistQuotesCount;
                stocktwits.CreatedOn = stocktwitsEntity.CreatedOn;

                return userSession;
            }

            if (userTypeId == (int)UserType.Fitbit)
            {
                if (!userSession.IsExpired && !userSession.IsAboutToExpire)
                    return userSession;

                //Update database
                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId) ?? throw new BadRequestException($"user consent is required");
                var userEntity = userSessionEntity.User;
                var fitbitEntity = userEntity.Fitbit!;

                var fitbitRefreshTokenResponse = await GetUserFitbitRefreshTokenResponseAsync(userSessionEntity.RefreshToken);
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                userSessionEntity.RefreshToken = fitbitRefreshTokenResponse.RefreshToken ?? userSessionEntity.RefreshToken;
                userSessionEntity.Token = fitbitRefreshTokenResponse.Token;
                userSessionEntity.TokenExpiresOn = currentTime.AddSeconds(fitbitRefreshTokenResponse.TokenDurationInSeconds);
                userSessionEntity.LastUpdatedOn = currentTime;

                if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.FitbitUpdateThresholdInHours)
                {
                    var fitbitResponse = await GetUserFitbitResponseAsync(userSessionEntity.Token);

                    userEntity.LastUpdatedOn = currentTime;

                    fitbitEntity.FirstName = fitbitResponse.FirstName;
                    fitbitEntity.LastName = fitbitResponse.LastName;
                    fitbitEntity.AgeInYears = fitbitResponse.AgeInYears;
                    fitbitEntity.HeightInCentimeters = fitbitResponse.HeightInCentimeters;
                    fitbitEntity.UserGenderId = fitbitResponse.Gender.ToUserGenderId();
                    fitbitEntity.CreatedOn = fitbitResponse.CreatedOn;
                }

                await _repository.SaveChangesAsync();

                //Update memory cache
                var user = IAccountMemoryCache.GetUser(userEntity.UserId);
                var fitbit = user.Fitbit!;

                userSession.Token = userSessionEntity.Token;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                fitbit.FirstName = fitbitEntity.FirstName;
                fitbit.LastName = fitbitEntity.LastName;
                fitbit.AgeInYears = fitbitEntity.AgeInYears;
                fitbit.HeightInCentimeters = fitbitEntity.HeightInCentimeters;
                fitbit.UserGenderId = fitbitEntity.UserGenderId;
                fitbit.CreatedOn = fitbitEntity.CreatedOn;

                return userSession;
            }

            throw new BadRequestException($"{nameof(userTypeId)} is invalid");
        }

        public async Task<AccountUserSessionContract> AuthenticateUserCodeAsync(int userTypeId, string code, string subdomain, string page, DateTimeZone timezone)
        {
            if (userTypeId == (int)UserType.Google)
            {
                var googleTokenResponse = await GetUserGoogleTokenResponseAsync(code, subdomain, page);
                var googleJsonWebToken = new JwtSecurityTokenHandler().ReadJwtToken(googleTokenResponse.UserJsonWebToken);
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                //Update database
                var userSessionEntity = new AccountUserSessionEntity
                {
                    Token = googleTokenResponse.Token,
                    RefreshToken = googleTokenResponse.RefreshToken,
                    TokenExpiresOn = currentTime.AddSeconds(googleTokenResponse.TokenDurationInSeconds),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };
                var userEntity = await _repository.GetUserAsync((int)UserType.Google, googleJsonWebToken.Subject) ?? await _repository.AddAndSaveChangesAsync(new AccountUserEntity
                {
                    UserTypeId = (int)UserType.Google,
                    ExternalUserId = googleJsonWebToken.Subject,
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime,
                    Google = new AccountUserGoogleEntity
                    {
                        Name = googleJsonWebToken.Payload.TryGetValue("name", out object? googleName)
                            ? googleName.ToString()!
                            : string.Empty,
                        Email = googleJsonWebToken.Payload.TryGetValue("email", out object? googleEmail)
                            ? googleEmail.ToString()!
                            : string.Empty
                    },
                    UserSessions = new List<AccountUserSessionEntity>
                    {
                        userSessionEntity
                    }
                });
                var googleEntity = userEntity.Google!;

                //Check for duplicate token
                var duplicateUserSessionEntity = userEntity.UserSessions.FirstOrDefault(existingUserSessionEntity => existingUserSessionEntity.Token == userSessionEntity.Token);

                if (duplicateUserSessionEntity is null)
                    userEntity.UserSessions.Add(userSessionEntity);
                else if (duplicateUserSessionEntity.UserSessionId != userSessionEntity.UserSessionId)
                {
                    duplicateUserSessionEntity.Token = userSessionEntity.Token;
                    duplicateUserSessionEntity.RefreshToken = userSessionEntity.RefreshToken;
                    duplicateUserSessionEntity.TokenExpiresOn = userSessionEntity.TokenExpiresOn;
                    duplicateUserSessionEntity.LastUpdatedOn = userSessionEntity.LastUpdatedOn;
                    userSessionEntity = duplicateUserSessionEntity;
                }

                //Check if user needs to be updated
                if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.GoogleUpdateThresholdInHours)
                {
                    userEntity.LastUpdatedOn = currentTime;

                    if (googleJsonWebToken.Payload.TryGetValue("name", out googleName))
                        googleEntity.Name = googleName.ToString()!;

                    if (googleJsonWebToken.Payload.TryGetValue("email", out googleEmail))
                        googleEntity.Email = googleEmail.ToString()!;
                }

                await _repository.SaveChangesAsync();

                //Update memory cache
                _userSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToUserSessionContract());
                _users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());

                var userSession = IAccountMemoryCache.GetUserSession(userSessionEntity.UserSessionId);
                var user = IAccountMemoryCache.GetUser(userEntity.UserId);
                var google = user.Google!;

                userSession.Token = userSessionEntity.Token;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                google.Name = googleEntity.Name;
                google.Username = googleEntity.Email;

                return userSession;
            }

            if (userTypeId == (int)UserType.Stocktwits)
            {
                var stocktwitsTokenResponse = await GetUserStocktwitsTokenResponseAsync(code, subdomain, page);
                var stocktwitsResponse = await GetUserStocktwitsResponseAsync(stocktwitsTokenResponse.Token);
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                //Update database
                var userSessionEntity = new AccountUserSessionEntity
                {
                    Token = stocktwitsTokenResponse.Token,
                    RefreshToken = string.Empty,
                    TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };
                var userEntity = await _repository.GetUserAsync((int)UserType.Stocktwits, stocktwitsResponse.UserId.ToString()) ?? await _repository.AddAndSaveChangesAsync(new AccountUserEntity
                {
                    UserTypeId = (int)UserType.Stocktwits,
                    ExternalUserId = stocktwitsResponse.UserId.ToString(),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime,
                    Stocktwits = new AccountUserStocktwitsEntity
                    {
                        Name = stocktwitsResponse.Name,
                        Username = stocktwitsResponse.Username,
                        FollowersCount = stocktwitsResponse.FollowersCount,
                        FollowingsCount = stocktwitsResponse.FollowingsCount,
                        PostsCount = stocktwitsResponse.PostsCount,
                        LikesCount = stocktwitsResponse.LikesCount,
                        WatchlistQuotesCount = stocktwitsResponse.WatchlistQuotesCount,
                        CreatedOn = stocktwitsResponse.CreatedOn
                    },
                    UserSessions = new List<AccountUserSessionEntity>
                    {
                        userSessionEntity
                    }
                });
                var stocktwitsEntity = userEntity.Stocktwits!;

                //Check for duplicate token
                var duplicateUserSessionEntity = userEntity.UserSessions.FirstOrDefault(existingUserSessionEntity => existingUserSessionEntity.Token == userSessionEntity.Token);

                if (duplicateUserSessionEntity is null)
                    userEntity.UserSessions.Add(userSessionEntity);
                else if (duplicateUserSessionEntity.UserSessionId != userSessionEntity.UserSessionId)
                {
                    duplicateUserSessionEntity.Token = userSessionEntity.Token;
                    duplicateUserSessionEntity.RefreshToken = userSessionEntity.RefreshToken;
                    duplicateUserSessionEntity.TokenExpiresOn = userSessionEntity.TokenExpiresOn;
                    duplicateUserSessionEntity.LastUpdatedOn = userSessionEntity.LastUpdatedOn;
                    userSessionEntity = duplicateUserSessionEntity;
                }

                //Check if user needs to be updated
                if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.StocktwitsUpdateThresholdInHours)
                {
                    userEntity.LastUpdatedOn = currentTime;

                    userEntity.Stocktwits!.Name = stocktwitsResponse.Name;
                    userEntity.Stocktwits.Username = stocktwitsResponse.Username;
                    userEntity.Stocktwits.FollowersCount = stocktwitsResponse.FollowersCount;
                    userEntity.Stocktwits.FollowingsCount = stocktwitsResponse.FollowingsCount;
                    userEntity.Stocktwits.PostsCount = stocktwitsResponse.PostsCount;
                    userEntity.Stocktwits.LikesCount = stocktwitsResponse.LikesCount;
                    userEntity.Stocktwits.WatchlistQuotesCount = stocktwitsResponse.WatchlistQuotesCount;
                    userEntity.Stocktwits.CreatedOn = stocktwitsResponse.CreatedOn;
                }

                await _repository.SaveChangesAsync();

                //Update memory cache
                _userSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToUserSessionContract());
                _users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());

                var userSession = IAccountMemoryCache.GetUserSession(userSessionEntity.UserSessionId);
                var user = IAccountMemoryCache.GetUser(userEntity.UserId);
                var stocktwits = user.Stocktwits!;

                userSession.Token = userSessionEntity.Token;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                stocktwits.Name = stocktwitsEntity.Name;
                stocktwits.Username = stocktwitsEntity.Username;
                stocktwits.FollowersCount = stocktwitsEntity.FollowersCount;
                stocktwits.FollowingsCount = stocktwitsEntity.FollowingsCount;
                stocktwits.PostsCount = stocktwitsEntity.PostsCount;
                stocktwits.LikesCount = stocktwitsEntity.LikesCount;
                stocktwits.WatchlistQuotesCount = stocktwitsEntity.WatchlistQuotesCount;
                stocktwits.CreatedOn = stocktwitsEntity.CreatedOn;

                return userSession;
            }

            if (userTypeId == (int)UserType.Fitbit)
            {
                var fitbitTokenResponse = await GetUserFitbitTokenResponseAsync(code, subdomain, page);
                var fitbitResponse = await GetUserFitbitResponseAsync(fitbitTokenResponse.Token);
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                //Update database
                var userSessionEntity = new AccountUserSessionEntity
                {
                    Token = fitbitTokenResponse.Token,
                    RefreshToken = fitbitTokenResponse.RefreshToken,
                    TokenExpiresOn = currentTime.AddSeconds(fitbitTokenResponse.TokenDurationInSeconds),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };
                var userEntity = await _repository.GetUserAsync((int)UserType.Fitbit, fitbitResponse.UserId) ?? await _repository.AddAndSaveChangesAsync(new AccountUserEntity
                {
                    UserTypeId = (int)UserType.Fitbit,
                    ExternalUserId = fitbitResponse.UserId,
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime,
                    Fitbit = new AccountUserFitbitEntity
                    {
                        UserGenderId = fitbitResponse.Gender.ToUserGenderId(),
                        FirstName = fitbitResponse.FirstName,
                        LastName = fitbitResponse.LastName,
                        AgeInYears = fitbitResponse.AgeInYears,
                        HeightInCentimeters = fitbitResponse.HeightInCentimeters,
                        CreatedOn = fitbitResponse.CreatedOn
                    },
                    UserSessions = new List<AccountUserSessionEntity>
                    {
                        userSessionEntity
                    }
                });
                var fitbitEntity = userEntity.Fitbit!;

                //Check for duplicate token
                var duplicateUserSessionEntity = userEntity.UserSessions.FirstOrDefault(existingUserSessionEntity => existingUserSessionEntity.Token == userSessionEntity.Token);

                if (duplicateUserSessionEntity is null)
                    userEntity.UserSessions.Add(userSessionEntity);
                else if (duplicateUserSessionEntity.UserSessionId != userSessionEntity.UserSessionId)
                {
                    duplicateUserSessionEntity.Token = userSessionEntity.Token;
                    duplicateUserSessionEntity.RefreshToken = userSessionEntity.RefreshToken;
                    duplicateUserSessionEntity.TokenExpiresOn = userSessionEntity.TokenExpiresOn;
                    duplicateUserSessionEntity.LastUpdatedOn = userSessionEntity.LastUpdatedOn;
                    userSessionEntity = duplicateUserSessionEntity;
                }

                //Check if user needs to be updated
                if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.FitbitUpdateThresholdInHours)
                {
                    userEntity.LastUpdatedOn = currentTime;

                    fitbitEntity.UserGenderId = fitbitResponse.Gender.ToUserGenderId();
                    fitbitEntity.FirstName = fitbitResponse.FirstName;
                    fitbitEntity.LastName = fitbitResponse.LastName;
                    fitbitEntity.AgeInYears = fitbitResponse.AgeInYears;
                    fitbitEntity.HeightInCentimeters = fitbitResponse.HeightInCentimeters;
                    fitbitEntity.CreatedOn = fitbitResponse.CreatedOn;
                }

                await _repository.SaveChangesAsync();

                //Update memory cache
                _userSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToUserSessionContract());
                _users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());

                var userSession = IAccountMemoryCache.GetUserSession(userSessionEntity.UserSessionId);
                var user = IAccountMemoryCache.GetUser(userEntity.UserId);
                var fitbit = user.Fitbit!;

                userSession.Token = userSessionEntity.Token;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                fitbit.UserGenderId = fitbitEntity.UserGenderId;
                fitbit.FirstName = fitbitEntity.FirstName;
                fitbit.LastName = fitbitEntity.LastName;
                fitbit.AgeInYears = fitbitEntity.AgeInYears;
                fitbit.HeightInCentimeters = fitbitEntity.HeightInCentimeters;
                fitbit.CreatedOn = fitbitEntity.CreatedOn;

                return userSession;
            }

            throw new BadRequestException($"{nameof(userTypeId)} is invalid");
        }

        #endregion

        #region Revoke

        public async Task RevokeUserSessionAsync(int userSessionId)
        {
            var userSession = IAccountMemoryCache.GetUserSession(userSessionId, isExpiredSessionValid: true);
            var user = IAccountMemoryCache.GetUser(userSession.UserId);

            if (user.UserTypeId == (int)UserType.Google)
            {
                await RevokeUserGoogleTokenResponseAsync(userSession.Token);

                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                if (userSessionEntity is not null)
                    await _repository.RemoveAsync(userSessionEntity);

                _userSessions.TryRemove(userSession.UserSessionId, out _);
            }

            if (user.UserTypeId == (int)UserType.Stocktwits)
            {
                return;

                //There is only one token per multiple devices, no need to revoke it
            }

            if (user.UserTypeId == (int)UserType.Fitbit)
            {
                await RevokeUserFitbitTokenResponseAsync(userSession.Token);

                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                if (userSessionEntity is not null)
                    await _repository.RemoveAsync(userSessionEntity);

                _userSessions.TryRemove(userSession.UserSessionId, out _);
            }
        }

        #endregion

        #region Google Api

        private string GetUserGoogleConsentUrlResponse(string subdomain, string page) =>
            _googleAuthenticationApi.BuildUri(
                new RestRequest("o/oauth2/v2/auth")
                    .AddQueryParameter("client_id", _configuration.GoogleApi.AuthenticationKey)
                    .AddQueryParameter("response_type", "code")
                    .AddQueryParameter("scope", "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile openid")
                    .AddQueryParameter("access_type", "offline")
                    .AddQueryParameter("prompt", "consent")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.UserAuthenticationRedirectUrl}?userTypeId={(int)UserType.Google}&subdomain={subdomain}&page={page}")
            ).AbsoluteUri;

        private async Task<AccountUserGoogleTokenResponse> GetUserGoogleTokenResponseAsync(string code, string subdomain, string page) =>
            (await _googleTokenApi.ExecutePostAsync<AccountUserGoogleTokenResponse>(
                new RestRequest("token")
                    .AddQueryParameter("client_id", _configuration.GoogleApi.AuthenticationKey)
                    .AddQueryParameter("client_secret", _configuration.GoogleApi.AuthenticationSecretKey)
                    .AddQueryParameter("grant_type", "authorization_code")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.UserAuthenticationRedirectUrl}?userTypeId={(int)UserType.Google}&subdomain={subdomain}&page={page}")
                    .AddQueryParameter("code", code)
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token) && !string.IsNullOrWhiteSpace(response?.RefreshToken));

        private async Task<AccountUserGoogleRefreshTokenResponse> GetUserGoogleRefreshTokenResponseAsync(string refreshToken) =>
            (await _googleTokenApi.ExecutePostAsync<AccountUserGoogleRefreshTokenResponse>(
                new RestRequest("token")
                    .AddQueryParameter("client_id", _configuration.GoogleApi.AuthenticationKey)
                    .AddQueryParameter("client_secret", _configuration.GoogleApi.AuthenticationSecretKey)
                    .AddQueryParameter("grant_type", "refresh_token")
                    .AddQueryParameter("refresh_token", refreshToken)
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

        private async Task<IRestResponse> RevokeUserGoogleTokenResponseAsync(string token) =>
            await _googleTokenApi.ExecutePostAsync(
                new RestRequest("revoke")
                    .AddQueryParameter("token", token)
            );

        #endregion

        #region Stocktwits Api

        private string GetUserStocktwitsConsentUrlResponse(string subdomain, string page) =>
            _stocktwitsApi.BuildUri(
                new RestRequest("oauth/authorize")
                    .AddQueryParameter("client_id", _configuration.StocktwitsApi.AuthenticationKey)
                    .AddQueryParameter("response_type", "code")
                    .AddQueryParameter("scope", "read,watch_lists")
                    .AddQueryParameter("prompt", "1")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.UserAuthenticationRedirectUrl}?userTypeId={(int)UserType.Stocktwits}&subdomain={subdomain}&page={page}")
            ).AbsoluteUri;

        private async Task<AccountUserStocktwitsTokenResponse> GetUserStocktwitsTokenResponseAsync(string code, string subdomain, string page) =>
            (await _stocktwitsApi.ExecutePostAsync<AccountUserStocktwitsTokenResponse>(
                new RestRequest("oauth/token")
                    .AddQueryParameter("client_id", _configuration.StocktwitsApi.AuthenticationKey)
                    .AddQueryParameter("client_secret", _configuration.StocktwitsApi.AuthenticationSecretKey)
                    .AddQueryParameter("grant_type", "authorization_code")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.UserAuthenticationRedirectUrl}?userTypeId={(int)UserType.Stocktwits}&subdomain={subdomain}&page={page}")
                    .AddQueryParameter("code", code)
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

        private async Task<AccountUserStocktwitsResponse> GetUserStocktwitsResponseAsync(string token) =>
            (await _stocktwitsApi.ExecuteGetAsync<AccountUserStocktwitsRootResponse>(
                new RestRequest("account/verify.json")
                    .AddQueryParameter("access_token", token)
            )).GetData(isSuccess: (response) => response?.User is not null).User;

        #endregion

        #region Fitbit Api

        private string GetUserFitbitConsentUrlResponse(string subdomain, string page) =>
            _fitbitAuthenticationApi.BuildUri(
                new RestRequest("oauth2/authorize")
                    .AddQueryParameter("client_id", _configuration.FitbitApi.AuthenticationKey)
                    .AddQueryParameter("response_type", "code")
                    .AddQueryParameter("scope", "activity location profile social")
                    .AddQueryParameter("prompt", "consent")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.UserAuthenticationRedirectUrl}?userTypeId={(int)UserType.Fitbit}&subdomain={subdomain}&page={page}")
            ).AbsoluteUri;

        private async Task<AccountUserFitbitTokenResponse> GetUserFitbitTokenResponseAsync(string code, string subdomain, string page) =>
            (await _fitbitTokenApi.ExecutePostAsync<AccountUserFitbitTokenResponse>(
                new RestRequest("oauth2/token")
                    .AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration.FitbitApi.AuthenticationKey}:{_configuration.FitbitApi.AuthenticationSecretKey}"))}")
                    .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                    .AddQueryParameter("client_id", _configuration.FitbitApi.AuthenticationKey)
                    .AddQueryParameter("grant_type", "authorization_code")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.UserAuthenticationRedirectUrl}?userTypeId={(int)UserType.Fitbit}&subdomain={subdomain}&page={page}")
                    .AddQueryParameter("code", code)
                    .AddQueryParameter("expires_in", "3600")
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token) && !string.IsNullOrWhiteSpace(response?.RefreshToken));

        private async Task<AccountUserFitbitRefreshTokenResponse> GetUserFitbitRefreshTokenResponseAsync(string refreshToken) =>
            (await _fitbitTokenApi.ExecutePostAsync<AccountUserFitbitRefreshTokenResponse>(
                new RestRequest("oauth2/token")
                    .AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration.FitbitApi.AuthenticationKey}:{_configuration.FitbitApi.AuthenticationSecretKey}"))}")
                    .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                    .AddQueryParameter("grant_type", "refresh_token")
                    .AddQueryParameter("refresh_token", refreshToken)
                    .AddQueryParameter("expires_in", "3600")
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

        private async Task<IRestResponse> RevokeUserFitbitTokenResponseAsync(string token) =>
            await _fitbitTokenApi.ExecutePostAsync(
                new RestRequest("oauth2/revoke")
                    .AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration.FitbitApi.AuthenticationKey}:{_configuration.FitbitApi.AuthenticationSecretKey}"))}")
                    .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                    .AddQueryParameter("token", token)
            );

        private async Task<AccountUserFitbitResponse> GetUserFitbitResponseAsync(string token) =>
            (await _fitbitTokenApi.ExecuteGetAsync<AccountUserFitbitResponseRoot>(
                new RestRequest("1/user/-/profile.json")
                    .AddHeader("Authorization", $"Bearer {token}")
            )).GetData(isSuccess: (response) => response?.User is not null).User;

        #endregion
    }

    public interface IAccountMemoryCache
    {
        Task MemoryCacheRefreshAsync(AccountDictionary? dictionary = null, int? id = null);

        public static IReadOnlyDictionary<int, AccountUserContract> Users => AccountService._users;
        public static IReadOnlyDictionary<int, AccountUserSessionContract> UserSessions => AccountService._userSessions;
        public static IReadOnlyDictionary<int, AccountUserTypeContract> UserTypes => AccountService._userTypes;
        public static IReadOnlyDictionary<int, AccountUserGenderContract> UserGenders => AccountService._userGenders;

        public static AccountUserContract GetUser(int userId) =>
            Users.TryGetValue(userId, out AccountUserContract? user)
                ? user
                : throw new BadRequestException($"{nameof(userId)} is invalid");

        public static AccountUserSessionContract GetUserSession(int userSessionId) =>
            UserSessions.TryGetValue(userSessionId, out AccountUserSessionContract? userSession)
                ? userSession
                : throw new BadRequestException($"{nameof(userSessionId)} is invalid");

        public static AccountUserSessionContract GetUserSession(int userSessionId, bool isExpiredSessionValid)
        {
            var userSession = GetUserSession(userSessionId);

            if (isExpiredSessionValid)
                return userSession;

            if (userSession.IsExpired)
                throw new BadRequestException($"{nameof(userSessionId)} is expired");

            return userSession;
        }

        public static AccountUserSessionContract GetUserSession(int userTypeId, string token) =>
            UserSessions.SingleOrDefault(userSession =>
                userSession.Value.Token == token &&
                userSession.Value.User.UserTypeId == userTypeId
            ).Value ?? throw new BadRequestException($"{nameof(userTypeId)}/{nameof(token)} is invalid");

        public static AccountUserSessionContract GetUserSession(int userTypeId, string token, bool isExpiredSessionValid)
        {
            var userSession = GetUserSession(userTypeId, token);

            if (isExpiredSessionValid)
                return userSession;

            if (userSession.IsExpired)
                throw new BadRequestException($"{nameof(userTypeId)}/{nameof(token)} is expired");

            return userSession;
        }

        public static AccountUserTypeContract GetUserType(int userTypeId) =>
            UserTypes.TryGetValue(userTypeId, out AccountUserTypeContract? userType)
                ? userType
                : throw new BadRequestException($"{nameof(userTypeId)} is invalid");

        public static AccountUserGenderContract GetUserGender(int userGenderId) =>
            UserGenders.TryGetValue(userGenderId, out AccountUserGenderContract? userGender)
                ? userGender
                : throw new BadRequestException($"{nameof(userGenderId)} is invalid");
    }

    public interface IAccountService : IAccountMemoryCache
    {
        Task<AccountUserSessionContract> AuthenticateUserCodeAsync(int userTypeId, string code, string subdomain, string page, DateTimeZone timezone);
        Task<AccountUserSessionContract> AuthenticateUserTokenAsync(int userTypeId, string token, DateTimeZone timezone);
        string GetUserConsentUrl(int userTypeId, string subdomain, string page);
        Task RevokeUserSessionAsync(int userSessionId);
        List<AccountUserContract> SearchUsers(string keyword, int userTypeId);
    }
}