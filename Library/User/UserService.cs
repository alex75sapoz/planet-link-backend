using Library.Base;
using Library.User.Contract;
using Library.User.Entity;
using Library.User.Enum;
using Library.User.Response;
using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Library.User
{
    public interface IUserService
    {
        Task<UserSessionContract> AuthenticateCodeAsync(int userTypeId, string code, string page, DateTimeZone timezone);
        Task<UserSessionContract> AuthenticateTokenAsync(int userTypeId, string token, DateTimeZone timezone);
        UserSessionContract GetSession(int userSessionId, bool isExpiredSessionValid = false);
        UserSessionContract GetSession(int userTypeId, string token, bool isExpiredSessionValid = false);
        UserContract GetUser(int userId);
        string GetUserConsentUrl(int userTypeId, string page);
        Task RevokeSessionAsync(int userSessionId);
        List<UserContract> SearchUsers(string keyword, int userTypeId);
    }

    internal class UserService : BaseService<UserConfiguration, UserRepository>, IUserService
    {
        public UserService(UserConfiguration configuration, UserRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache)
        {
            _googleAuthenticationApi = new RestClient(_configuration.GoogleApi.AuthenticationServer);
            _googleTokenApi = new RestClient(_configuration.GoogleApi.TokenServer);
            _stocktwitsApi = new RestClient(_configuration.StocktwitsApi.Server);
        }

        private readonly IRestClient _googleAuthenticationApi;
        private readonly IRestClient _googleTokenApi;
        private readonly IRestClient _stocktwitsApi;

        #region Search

        public List<UserContract> SearchUsers(string keyword, int userTypeId) => (userTypeId switch
        {
            (int)UserType.Google => UserMemoryCache.Users.Where(user =>
                user.Value.Type.TypeId == userTypeId &&
                user.Value.Google.Username.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
            ),
            (int)UserType.Stocktwits => UserMemoryCache.Users.Where(user =>
                user.Value.Type.TypeId == userTypeId &&
                user.Value.Stocktwits.Username.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
            ),
            _ => throw new BadRequestException($"{nameof(userTypeId)} is invalid")
        }).Select(user => user.Value)
          .Take(_configuration.Limit.SearchStocktwitsLimit)
          .ToList();


        #endregion

        #region Get

        public string GetUserConsentUrl(int userTypeId, string page) => userTypeId switch
        {
            (int)UserType.Google => GetGoogleConsentUrlResponse(page),
            (int)UserType.Stocktwits => GetStocktwitsConsentUrlResponse(page),
            _ => throw new BadRequestException($"{nameof(userTypeId)} is invalid")
        };

        public UserContract GetUser(int userId) =>
            UserMemoryCache.Users.TryGetValue(userId, out UserContract user)
                ? user
                : throw new BadRequestException($"{nameof(userId)} is invalid");

        public UserSessionContract GetSession(int userSessionId, bool isExpiredSessionValid = false) =>
            UserMemoryCache.UserSessions.TryGetValue(userSessionId, out UserSessionContract userSession)
                ? isExpiredSessionValid || !userSession.IsExpired
                      ? userSession
                      : throw new BadRequestException($"{nameof(userSessionId)} is expired")
                : throw new BadRequestException($"{nameof(userSessionId)} is invalid");

        public UserSessionContract GetSession(int userTypeId, string token, bool isExpiredSessionValid = false)
        {
            var userSession = UserMemoryCache.UserSessions.SingleOrDefault(userSession =>
                userSession.Value.Token == token &&
                userSession.Value.User.Type.TypeId == userTypeId
            ).Value ?? throw new BadRequestException($"{nameof(userTypeId)}/{nameof(token)} is invalid");

            if (!isExpiredSessionValid && userSession.IsExpired)
                throw new BadRequestException($"{nameof(token)} is expired");

            return userSession;
        }

        #endregion

        #region Authenticate

        public async Task<UserSessionContract> AuthenticateTokenAsync(int userTypeId, string token, DateTimeZone timezone)
        {
            var userSession = GetSession(userTypeId, token, isExpiredSessionValid: true);

            if (userTypeId == (int)UserType.Google)
            {
                if (userSession.IsExpired || userSession.IsAboutToExpire)
                {
                    var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                    if (userSessionEntity is null)
                    {
                        UserMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
                        throw new BadRequestException($"user consent is required");
                    }

                    var googleRefreshTokenResponse = await GetGoogleRefreshTokenResponseAsync(userSession.RefreshToken);
                    var googleJsonWebToken = new JwtSecurityTokenHandler().ReadJwtToken(googleRefreshTokenResponse.UserJsonWebToken);
                    var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                    if ((currentTime - userSessionEntity.User.LastUpdatedOn).TotalHours > _configuration.Threshold.GoogleUpdateThresholdInHours)
                    {
                        userSessionEntity.User.Google.Name = googleJsonWebToken.Payload.TryGetValue("name", out object name) ? name.ToString() : userSessionEntity.User.Google.Name;
                        userSessionEntity.User.Google.Email = googleJsonWebToken.Payload.TryGetValue("email", out object email) ? email.ToString() : userSessionEntity.User.Google.Email;
                        userSessionEntity.User.LastUpdatedOn = currentTime;
                    }

                    userSessionEntity.Token = googleRefreshTokenResponse.Token;
                    userSessionEntity.TokenExpiresOn = currentTime.AddSeconds(googleRefreshTokenResponse.TokenDurationInSeconds);
                    userSessionEntity.LastUpdatedOn = currentTime;

                    await _repository.SaveChangesAsync();

                    userSession.User.Google.Name = userSessionEntity.User.Google.Name;
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
                        UserMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
                        throw new BadRequestException("user consent is required");
                    }

                    var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                    if ((currentTime - userSessionEntity.User.LastUpdatedOn).TotalHours > _configuration.Threshold.StocktwitsUpdateThresholdInHours)
                    {
                        var userStocktwitsResponse = await GetStocktwitsResponseAsync(userSession.Token);

                        userSessionEntity.User.Stocktwits.Name = userStocktwitsResponse.Name;
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

                    userSession.User.Stocktwits.Name = userSessionEntity.User.Stocktwits.Name;
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

        public async Task<UserSessionContract> AuthenticateCodeAsync(int userTypeId, string code, string page, DateTimeZone timezone)
        {
            if (userTypeId == (int)UserType.Google)
            {
                var googleTokenResponse = await GetGoogleTokenResponseAsync(code, page);
                var googleJsonWebToken = new JwtSecurityTokenHandler().ReadJwtToken(googleTokenResponse.UserJsonWebToken);
                var userEntity = await _repository.GetUserAsync((int)UserType.Google, googleJsonWebToken.Subject);

                if (userEntity is null && string.IsNullOrWhiteSpace(googleTokenResponse.RefreshToken))
                    throw new BadRequestException("user consent is required");

                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                var userSessionEntity = new UserSessionEntity
                {
                    Token = googleTokenResponse.Token,
                    RefreshToken = googleTokenResponse.RefreshToken,
                    TokenExpiresOn = currentTime.AddSeconds(googleTokenResponse.TokenDurationInSeconds),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };

                if (userEntity is null)
                {
                    userEntity = await _repository.AddAndSaveChangesAsync(new UserEntity
                    {
                        UserTypeId = (int)UserType.Google,
                        ExternalUserId = googleJsonWebToken.Subject,
                        CreatedOn = currentTime,
                        LastUpdatedOn = currentTime,
                        Google = new UserGoogleEntity
                        {
                            Name = googleJsonWebToken.Payload.TryGetValue("name", out object name) ? name.ToString() : string.Empty,
                            Email = googleJsonWebToken.Payload.TryGetValue("email", out object email) ? email.ToString() : string.Empty
                        },
                        Sessions = new List<UserSessionEntity>
                        {
                            userSessionEntity
                        }
                    });
                }
                else
                {
                    if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.GoogleUpdateThresholdInHours)
                    {
                        userEntity.Google.Name = googleJsonWebToken.Payload.TryGetValue("name", out object name) ? name.ToString() : userEntity.Google.Name;
                        userEntity.Google.Email = googleJsonWebToken.Payload.TryGetValue("email", out object email) ? email.ToString() : userEntity.Google.Email;
                        userEntity.LastUpdatedOn = currentTime;
                    }

                    var existingSessionEntity = userEntity.Sessions.FirstOrDefault(userSession => userSession.Token == userSessionEntity.Token);

                    if (existingSessionEntity is null)
                        userEntity.Sessions.Add(userSessionEntity);
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

                UserMemoryCache.Users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());
                UserMemoryCache.UserSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToSessionContract());

                var userSession = UserMemoryCache.UserSessions[userSessionEntity.UserSessionId];

                userSession.User.Google.Name = userEntity.Google.Name;
                userSession.User.Google.Username = userEntity.Google.Email;
                userSession.Token = userSessionEntity.Token;
                userSession.RefreshToken = userSessionEntity.RefreshToken;
                userSession.TokenExpiresOn = userSessionEntity.TokenExpiresOn;

                return userSession;
            }

            if (userTypeId == (int)UserType.Stocktwits)
            {
                var stocktwitsTokenResponse = await GetStocktwitsTokenResponseAsync(code, page);
                var userEntity = await _repository.GetUserAsync((int)UserType.Stocktwits, stocktwitsTokenResponse.UserId.ToString());
                var currentTime = DateTimeOffset.Now.AtTimezone(timezone);

                var userSessionEntity = new UserSessionEntity
                {
                    Token = stocktwitsTokenResponse.Token,
                    RefreshToken = string.Empty,
                    TokenExpiresOn = currentTime.AddHours(_configuration.Duration.StocktwitsTokenDurationInHours),
                    CreatedOn = currentTime,
                    LastUpdatedOn = currentTime
                };

                if (userEntity is null)
                {
                    var userStocktwitsResponse = await GetStocktwitsResponseAsync(stocktwitsTokenResponse.Token);

                    userEntity = await _repository.AddAndSaveChangesAsync(new UserEntity
                    {
                        UserTypeId = (int)UserType.Stocktwits,
                        ExternalUserId = stocktwitsTokenResponse.UserId.ToString(),
                        CreatedOn = currentTime,
                        LastUpdatedOn = currentTime,
                        Stocktwits = new UserStocktwitsEntity
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
                        Sessions = new List<UserSessionEntity>
                        {
                            userSessionEntity
                        }
                    });
                }
                else
                {
                    if ((currentTime - userEntity.LastUpdatedOn).TotalHours > _configuration.Threshold.StocktwitsUpdateThresholdInHours)
                    {
                        var userStocktwitsResponse = await GetStocktwitsResponseAsync(stocktwitsTokenResponse.Token);
                        userEntity.Stocktwits.Name = userStocktwitsResponse.Name;
                        userEntity.Stocktwits.Username = userStocktwitsResponse.Username;
                        userEntity.Stocktwits.FollowersCount = userStocktwitsResponse.FollowersCount;
                        userEntity.Stocktwits.FollowingsCount = userStocktwitsResponse.FollowingsCount;
                        userEntity.Stocktwits.PostsCount = userStocktwitsResponse.PostsCount;
                        userEntity.Stocktwits.LikesCount = userStocktwitsResponse.LikesCount;
                        userEntity.Stocktwits.WatchlistQuotesCount = userStocktwitsResponse.WatchlistQuotesCount;
                        userEntity.Stocktwits.CreatedOn = userStocktwitsResponse.CreatedOn;
                        userEntity.LastUpdatedOn = currentTime;
                    }

                    var existingSession = userEntity.Sessions.FirstOrDefault(userSession => userSession.Token == userSessionEntity.Token);

                    if (existingSession is null)
                        userEntity.Sessions.Add(userSessionEntity);
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

                UserMemoryCache.Users.TryAdd(userEntity.UserId, userEntity.MapToUserContract());
                UserMemoryCache.UserSessions.TryAdd(userSessionEntity.UserSessionId, userSessionEntity.MapToSessionContract());

                var userSession = UserMemoryCache.UserSessions[userSessionEntity.UserSessionId];

                userSession.User.Stocktwits.Name = userEntity.Stocktwits.Name;
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

        public async Task RevokeSessionAsync(int userSessionId)
        {
            var userSession = GetSession(userSessionId, isExpiredSessionValid: true);

            if (userSession.User.Type.TypeId == (int)UserType.Google)
            {
                await RevokeGoogleTokenResponseAsync(userSession.Token);

                var userSessionEntity = await _repository.GetUserSessionAsync(userSession.UserSessionId);

                if (userSessionEntity is not null)
                    await _repository.RemoveAsync(userSessionEntity);

                UserMemoryCache.UserSessions.TryRemove(userSession.UserSessionId, out _);
            }

            if (userSession.User.Type.TypeId == (int)UserType.Stocktwits)
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

        private string GetGoogleConsentUrlResponse(string page) =>
            _googleAuthenticationApi.BuildUri(
                new RestRequest("o/oauth2/v2/auth")
                    .AddQueryParameter("client_id", _configuration.GoogleApi.AuthenticationKey)
                    .AddQueryParameter("response_type", "code")
                    .AddQueryParameter("scope", "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile openid")
                    .AddQueryParameter("access_type", "offline")
                    .AddQueryParameter("prompt", "consent")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.RedirectUrl}?userTypeId={(int)UserType.Google}&page={page}")
            ).AbsoluteUri;

        private async Task<UserGoogleTokenResponse> GetGoogleTokenResponseAsync(string code, string page) =>
            (await _googleTokenApi.ExecutePostAsync<UserGoogleTokenResponse>(
                new RestRequest("token")
                    .AddQueryParameter("client_id", _configuration.GoogleApi.AuthenticationKey)
                    .AddQueryParameter("client_secret", _configuration.GoogleApi.AuthenticationSecretKey)
                    .AddQueryParameter("grant_type", "authorization_code")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.RedirectUrl}?userTypeId={(int)UserType.Google}&page={page}")
                    .AddQueryParameter("code", code)
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

        private async Task<UserGoogleRefreshTokenResponse> GetGoogleRefreshTokenResponseAsync(string refreshToken) =>
            (await _googleTokenApi.ExecutePostAsync<UserGoogleRefreshTokenResponse>(
                new RestRequest("token")
                    .AddQueryParameter("client_id", _configuration.GoogleApi.AuthenticationKey)
                    .AddQueryParameter("client_secret", _configuration.GoogleApi.AuthenticationSecretKey)
                    .AddQueryParameter("grant_type", "refresh_token")
                    .AddQueryParameter("refresh_token", refreshToken)
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

        private async Task<IRestResponse> RevokeGoogleTokenResponseAsync(string token) =>
            await _googleTokenApi.ExecutePostAsync(
                new RestRequest("revoke")
                    .AddQueryParameter("token", token)
            );

        #endregion

        #region Stocktwits Api

        private string GetStocktwitsConsentUrlResponse(string page) =>
            _stocktwitsApi.BuildUri(
                new RestRequest("oauth/authorize")
                    .AddQueryParameter("client_id", _configuration.StocktwitsApi.AuthenticationKey)
                    .AddQueryParameter("response_type", "code")
                    .AddQueryParameter("scope", "read,watch_lists")
                    .AddQueryParameter("prompt", "1")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.RedirectUrl}?userTypeId={(int)UserType.Stocktwits}&page={page}")
            ).AbsoluteUri;

        private async Task<UserStocktwitsTokenResponse> GetStocktwitsTokenResponseAsync(string code, string page) =>
            (await _stocktwitsApi.ExecutePostAsync<UserStocktwitsTokenResponse>(
                new RestRequest("oauth/token")
                    .AddQueryParameter("client_id", _configuration.StocktwitsApi.AuthenticationKey)
                    .AddQueryParameter("client_secret", _configuration.StocktwitsApi.AuthenticationSecretKey)
                    .AddQueryParameter("grant_type", "authorization_code")
                    .AddQueryParameter("redirect_uri", $"{_configuration.Default.RedirectUrl}?userTypeId={(int)UserType.Stocktwits}&page={page}")
                    .AddQueryParameter("code", code)
            )).GetData(isSuccess: (response) => !string.IsNullOrWhiteSpace(response?.Token));

        private async Task<UserStocktwitsResponse> GetStocktwitsResponseAsync(string token) =>
            (await _stocktwitsApi.ExecuteGetAsync<StocktwitsUserResponseRoot>(
                new RestRequest("account/verify.json")
                    .AddQueryParameter("access_token", token)
            )).GetData(isSuccess: (response) => response?.User is not null).User;

        #endregion
    }
}