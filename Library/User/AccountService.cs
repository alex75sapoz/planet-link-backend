using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Account
{
    public interface IAccountService
    {
        Task<AccountUserSessionContract> AuthenticateUserCodeAsync(int userTypeId, string code, string subdomain, string page, DateTimeZone timezone);
        Task<AccountUserSessionContract> AuthenticateUserTokenAsync(int userTypeId, string token, DateTimeZone timezone);
        AccountUserSessionContract GetUserSession(int userSessionId, bool isExpiredSessionValid = false);
        AccountUserSessionContract GetUserSession(int userTypeId, string token, bool isExpiredSessionValid = false);
        AccountUserContract GetUser(int userId);
        string GetUserConsentUrl(int userTypeId, string subdomain, string page);
        Task RevokeUserSessionAsync(int userSessionId);
        List<AccountUserContract> SearchUsers(string keyword, int userTypeId);
    }

    class AccountService : BaseService<AccountConfiguration, AccountRepository>, IAccountService
    {
        public AccountService(AccountConfiguration configuration, AccountRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache)
        {
            _googleAuthenticationApi = new RestClient(_configuration.GoogleApi.AuthenticationServer);
            _googleTokenApi = new RestClient(_configuration.GoogleApi.TokenServer);
            _stocktwitsApi = new RestClient(_configuration.StocktwitsApi.Server);
        }

        private readonly IRestClient _googleAuthenticationApi;
        private readonly IRestClient _googleTokenApi;
        private readonly IRestClient _stocktwitsApi;

        #region Search

        public List<AccountUserContract> SearchUsers(string keyword, int userTypeId) => (userTypeId switch
        {
            (int)UserType.Google => AccountMemoryCache.Users.Where(user =>
                user.Value.UserTypeId == userTypeId &&
                user.Value.Google!.Username.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
            ),
            (int)UserType.Stocktwits => AccountMemoryCache.Users.Where(user =>
                user.Value.UserTypeId == userTypeId &&
                user.Value.Stocktwits!.Username.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
            ),
            _ => throw new BadRequestException($"{nameof(userTypeId)} is invalid")
        }).Select(user => user.Value)
          .Take(_configuration.Limit.SearchUsersLimit)
          .ToList();


        #endregion

        #region Get

        public string GetUserConsentUrl(int userTypeId, string subdomain, string page) => userTypeId switch
        {
            (int)UserType.Google => GetUserGoogleConsentUrlResponse(subdomain, page),
            (int)UserType.Stocktwits => GetUserStocktwitsConsentUrlResponse(subdomain, page),
            _ => throw new BadRequestException($"{nameof(userTypeId)} is invalid")
        };

        public AccountUserContract GetUser(int userId) =>
            AccountMemoryCache.Users.TryGetValue(userId, out AccountUserContract? user)
                ? user
                : throw new BadRequestException($"{nameof(userId)} is invalid");

        public AccountUserSessionContract GetUserSession(int userSessionId, bool isExpiredSessionValid = false) =>
            AccountMemoryCache.UserSessions.TryGetValue(userSessionId, out AccountUserSessionContract? userSession)
                ? isExpiredSessionValid || !userSession.IsExpired
                      ? userSession
                      : throw new BadRequestException($"{nameof(userSessionId)} is expired")
                : throw new BadRequestException($"{nameof(userSessionId)} is invalid");

        public AccountUserSessionContract GetUserSession(int userTypeId, string token, bool isExpiredSessionValid = false)
        {
            var userSession = AccountMemoryCache.UserSessions.SingleOrDefault(userSession =>
                userSession.Value.Token == token &&
                userSession.Value.User.UserTypeId == userTypeId
            ).Value ?? throw new BadRequestException($"{nameof(userTypeId)}/{nameof(token)} is invalid");

            if (!isExpiredSessionValid && userSession.IsExpired)
                throw new BadRequestException($"{nameof(token)} is expired");

            return userSession;
        }

        #endregion

        #region Authenticate

        public async Task<AccountUserSessionContract> AuthenticateUserTokenAsync(int userTypeId, string token, DateTimeZone timezone)
        {
            var userSession = GetUserSession(userTypeId, token, isExpiredSessionValid: true);

            if (userTypeId == (int)UserType.Google)
            {
                if (userSession.IsExpired || userSession.IsAboutToExpire)
                {
                    var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                    if (userSessionEntity is null)
                    {
                        AccountMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
                        throw new BadRequestException($"user consent is required");
                    }

                    var googleRefreshTokenResponse = await GetUserGoogleRefreshTokenResponseAsync(userSession.RefreshToken);
                    var googleJsonWebToken = new JwtSecurityTokenHandler().ReadJwtToken(googleRefreshTokenResponse.UserJsonWebToken);
                    var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                    if ((currentTime - userSessionEntity.User.LastUpdatedOn).TotalHours > _configuration.Threshold.GoogleUpdateThresholdInHours)
                    {
                        userSessionEntity.User.Google!.Name = googleJsonWebToken.Payload.TryGetValue("name", out object? name) ? name.ToString()! : userSessionEntity.User.Google.Name;
                        userSessionEntity.User.Google.Email = googleJsonWebToken.Payload.TryGetValue("email", out object? email) ? email.ToString()! : userSessionEntity.User.Google.Email;
                        userSessionEntity.User.LastUpdatedOn = currentTime;
                    }

                    userSessionEntity.Token = googleRefreshTokenResponse.Token;
                    userSessionEntity.TokenExpiresOn = currentTime.AddSeconds(googleRefreshTokenResponse.TokenDurationInSeconds);
                    userSessionEntity.LastUpdatedOn = currentTime;

                    await _repository.SaveChangesAsync();

                    userSession.User.Google!.Name = userSessionEntity.User.Google!.Name;
                    userSession.User.Google.Username = userSessionEntity.User.Google.Email;
                    userSession.Token = userSessionEntity.Token;
                    userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;
                }

                return userSession;
            }

            if (userTypeId == (int)UserType.Stocktwits)
            {
                if (userSession.IsExpired || userSession.IsAboutToExpire)
                {
                    var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                    if (userSessionEntity is null)
                    {
                        AccountMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
                        throw new BadRequestException("user consent is required");
                    }

                    var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                    if ((currentTime - userSessionEntity.User.LastUpdatedOn).TotalHours > _configuration.Threshold.StocktwitsUpdateThresholdInHours)
                    {
                        var userStocktwitsResponse = await GetUserStocktwitsResponseAsync(userSession.Token);

                        userSessionEntity.User.Stocktwits!.Name = userStocktwitsResponse.Name;
                        userSessionEntity.User.Stocktwits.Username = userStocktwitsResponse.Username;
                        userSessionEntity.User.Stocktwits.FollowersCount = userStocktwitsResponse.FollowersCount;
                        userSessionEntity.User.Stocktwits.FollowingsCount = userStocktwitsResponse.FollowingsCount;
                        userSessionEntity.User.Stocktwits.PostsCount = userStocktwitsResponse.PostsCount;
                        userSessionEntity.User.Stocktwits.LikesCount = userStocktwitsResponse.LikesCount;
                        userSessionEntity.User.Stocktwits.WatchlistQuotesCount = userStocktwitsResponse.WatchlistQuotesCount;
                        userSessionEntity.User.Stocktwits.CreatedOn = userStocktwitsResponse.CreatedOn;
                        userSessionEntity.User.LastUpdatedOn = currentTime;
                    }

                    userSessionEntity.TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours);
                    userSessionEntity.LastUpdatedOn = currentTime;

                    await _repository.SaveChangesAsync();

                    userSession.User.Stocktwits!.Name = userSessionEntity.User.Stocktwits!.Name;
                    userSession.User.Stocktwits.Username = userSessionEntity.User.Stocktwits.Username;
                    userSession.User.Stocktwits.FollowersCount = userSessionEntity.User.Stocktwits.FollowersCount;
                    userSession.User.Stocktwits.FollowingsCount = userSessionEntity.User.Stocktwits.FollowingsCount;
                    userSession.User.Stocktwits.PostsCount = userSessionEntity.User.Stocktwits.PostsCount;
                    userSession.User.Stocktwits.LikesCount = userSessionEntity.User.Stocktwits.LikesCount;
                    userSession.User.Stocktwits.WatchlistQuotesCount = userSessionEntity.User.Stocktwits.WatchlistQuotesCount;
                    userSession.User.Stocktwits.CreatedOn = userSessionEntity.User.Stocktwits.CreatedOn;
                    userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;
                }

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
                var userEntity = await _repository.GetUserAsync((int)UserType.Google, googleJsonWebToken.Subject);

                if (userEntity is null && string.IsNullOrWhiteSpace(googleTokenResponse.RefreshToken))
                    throw new BadRequestException("user consent is required");

                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                var userSessionEntity = new AccountUserSessionEntity
                {
                    Token = googleTokenResponse.Token,
                    RefreshToken = googleTokenResponse.RefreshToken,
                    TokenExpiresOn = currentTime.AddSeconds(googleTokenResponse.TokenDurationInSeconds),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };

                if (userEntity is null)
                {
                    userEntity = await _repository.AddAndSaveChangesAsync(new AccountUserEntity
                    {
                        UserTypeId = (int)UserType.Google,
                        ExternalUserId = googleJsonWebToken.Subject,
                        CreatedOn = currentTime,
                        LastUpdatedOn = currentTime,
                        Google = new AccountUserGoogleEntity
                        {
                            Name = googleJsonWebToken.Payload.TryGetValue("name", out object? name) ? name.ToString()! : string.Empty,
                            Email = googleJsonWebToken.Payload.TryGetValue("email", out object? email) ? email.ToString()! : string.Empty
                        },
                        UserSessions = new List<AccountUserSessionEntity>
                        {
                            userSessionEntity
                        }
                    });
                }
                else
                {
                    if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.GoogleUpdateThresholdInHours)
                    {
                        userEntity.Google!.Name = googleJsonWebToken.Payload.TryGetValue("name", out object? name) ? name.ToString()! : userEntity.Google.Name;
                        userEntity.Google.Email = googleJsonWebToken.Payload.TryGetValue("email", out object? email) ? email.ToString()! : userEntity.Google.Email;
                        userEntity.LastUpdatedOn = currentTime;
                    }

                    var existingSessionEntity = userEntity.UserSessions.FirstOrDefault(userSession => userSession.Token == userSessionEntity.Token);

                    if (existingSessionEntity is null)
                        userEntity.UserSessions.Add(userSessionEntity);
                    else
                    {
                        existingSessionEntity.Token = googleTokenResponse.Token;
                        existingSessionEntity.RefreshToken = googleTokenResponse.RefreshToken;
                        existingSessionEntity.TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours);
                        existingSessionEntity.LastUpdatedOn = currentTime;
                        userSessionEntity = existingSessionEntity;
                    }

                    await _repository.SaveChangesAsync();
                }

                AccountMemoryCache.Users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());
                AccountMemoryCache.UserSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToUserSessionContract());

                var userSession = AccountMemoryCache.UserSessions[userSessionEntity.UserSessionId];

                userSession.User.Google!.Name = userEntity.Google!.Name;
                userSession.User.Google.Username = userEntity.Google.Email;
                userSession.Token = userSessionEntity.Token;
                userSession.RefreshToken = userSessionEntity.RefreshToken;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                return userSession;
            }

            if (userTypeId == (int)UserType.Stocktwits)
            {
                var stocktwitsTokenResponse = await GetUserStocktwitsTokenResponseAsync(code, subdomain, page);
                var userEntity = await _repository.GetUserAsync((int)UserType.Stocktwits, stocktwitsTokenResponse.UserId.ToString());
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                var userSessionEntity = new AccountUserSessionEntity
                {
                    Token = stocktwitsTokenResponse.Token,
                    RefreshToken = string.Empty,
                    TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };

                if (userEntity is null)
                {
                    var userStocktwitsResponse = await GetUserStocktwitsResponseAsync(stocktwitsTokenResponse.Token);

                    userEntity = await _repository.AddAndSaveChangesAsync(new AccountUserEntity
                    {
                        UserTypeId = (int)UserType.Stocktwits,
                        ExternalUserId = stocktwitsTokenResponse.UserId.ToString(),
                        CreatedOn = currentTime,
                        LastUpdatedOn = currentTime,
                        Stocktwits = new AccountUserStocktwitsEntity
                        {
                            Name = userStocktwitsResponse.Name,
                            Username = userStocktwitsResponse.Username,
                            FollowersCount = userStocktwitsResponse.FollowersCount,
                            FollowingsCount = userStocktwitsResponse.FollowingsCount,
                            PostsCount = userStocktwitsResponse.PostsCount,
                            LikesCount = userStocktwitsResponse.LikesCount,
                            WatchlistQuotesCount = userStocktwitsResponse.WatchlistQuotesCount,
                            CreatedOn = userStocktwitsResponse.CreatedOn
                        },
                        UserSessions = new List<AccountUserSessionEntity>
                        {
                            userSessionEntity
                        }
                    });
                }
                else
                {
                    if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.StocktwitsUpdateThresholdInHours)
                    {
                        var userStocktwitsResponse = await GetUserStocktwitsResponseAsync(stocktwitsTokenResponse.Token);
                        userEntity.Stocktwits!.Name = userStocktwitsResponse.Name;
                        userEntity.Stocktwits.Username = userStocktwitsResponse.Username;
                        userEntity.Stocktwits.FollowersCount = userStocktwitsResponse.FollowersCount;
                        userEntity.Stocktwits.FollowingsCount = userStocktwitsResponse.FollowingsCount;
                        userEntity.Stocktwits.PostsCount = userStocktwitsResponse.PostsCount;
                        userEntity.Stocktwits.LikesCount = userStocktwitsResponse.LikesCount;
                        userEntity.Stocktwits.WatchlistQuotesCount = userStocktwitsResponse.WatchlistQuotesCount;
                        userEntity.Stocktwits.CreatedOn = userStocktwitsResponse.CreatedOn;
                        userEntity.LastUpdatedOn = currentTime;
                    }

                    var existingSession = userEntity.UserSessions.FirstOrDefault(userSession => userSession.Token == userSessionEntity.Token);

                    if (existingSession is null)
                        userEntity.UserSessions.Add(userSessionEntity);
                    else
                    {
                        existingSession.Token = stocktwitsTokenResponse.Token;
                        existingSession.RefreshToken = string.Empty;
                        existingSession.TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours);
                        existingSession.LastUpdatedOn = currentTime;
                        userSessionEntity = existingSession;
                    }

                    await _repository.SaveChangesAsync();
                }

                AccountMemoryCache.Users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());
                AccountMemoryCache.UserSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToUserSessionContract());

                var userSession = AccountMemoryCache.UserSessions[userSessionEntity.UserSessionId];

                userSession.User.Stocktwits!.Name = userEntity.Stocktwits!.Name;
                userSession.User.Stocktwits.Username = userEntity.Stocktwits.Username;
                userSession.User.Stocktwits.FollowersCount = userEntity.Stocktwits.FollowersCount;
                userSession.User.Stocktwits.FollowingsCount = userEntity.Stocktwits.FollowingsCount;
                userSession.User.Stocktwits.PostsCount = userEntity.Stocktwits.PostsCount;
                userSession.User.Stocktwits.LikesCount = userEntity.Stocktwits.LikesCount;
                userSession.User.Stocktwits.WatchlistQuotesCount = userEntity.Stocktwits.WatchlistQuotesCount;
                userSession.User.Stocktwits.CreatedOn = userEntity.Stocktwits.CreatedOn;
                userSession.Token = userSessionEntity.Token;
                userSession.RefreshToken = userSessionEntity.RefreshToken;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                return userSession;
            }

            throw new BadRequestException($"{nameof(userTypeId)} is invalid");
        }

        #endregion

        #region Revoke

        public async Task RevokeUserSessionAsync(int userSessionId)
        {
            var userSession = GetUserSession(userSessionId, isExpiredSessionValid: true);

            if (userSession.User.UserTypeId == (int)UserType.Google)
            {
                await RevokeUserGoogleTokenResponseAsync(userSession.Token);

                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                if (userSessionEntity is not null)
                    await _repository.RemoveAsync(userSessionEntity);

                AccountMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
            }

            if (userSession.User.UserTypeId == (int)UserType.Stocktwits)
            {
                //There is only one token per multiple devices, no need to revoke it
                return;

                //var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                //if (userSessionEntity is not null)
                //    await _repository.RemoveAsync(userSessionEntity);

                //UserMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
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
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

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
            (await _stocktwitsApi.ExecuteGetAsync<StocktwitsUserResponseRoot>(
                new RestRequest("account/verify.json")
                    .AddQueryParameter("access_token", token)
            )).GetData(isSuccess: (response) => response?.User is not null).User;

        #endregion
    }
}