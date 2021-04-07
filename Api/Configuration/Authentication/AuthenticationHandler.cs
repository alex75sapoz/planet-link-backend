using Api.Configuration.MemoryCache;
using Api.Controller;
using Api.Library.User;
using Api.Library.User.Contract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NodaTime;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Api.Configuration.Authentication
{
    internal class AuthenticationHandler : AuthenticationHandler<AuthenticationScheme>
    {
        public AuthenticationHandler(IOptionsMonitor<AuthenticationScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserService userService) : base(options, logger, encoder, clock) =>
            _userService = userService;

        private readonly IUserService _userService;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Swagger is publicly available
            if (Options.IsDevelopment && Request.Path.StartsWithSegments(Options.SwaggerUrlSegment))
                return GetSwaggerAuthenticateResult();

            //Step 1 - Memory Cache
            if (!MemoryCacheRefreshJob.IsReady)
                return AuthenticateResult.Fail($"{nameof(MemoryCacheRefreshJob)} is not ready");

            //Step 2 - TimezoneId
            if (!Request.Headers.TryGetValue(ApiHeader.TimezoneId, out StringValues headerTimezoneId))
                return AuthenticateResult.Fail($"{ApiHeader.TimezoneId} is required");

            var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(headerTimezoneId);

            if (timezone is null)
                return AuthenticateResult.Fail($"{ApiHeader.TimezoneId} is invalid");

            //Step 3 - UserTypeId
            if (!Request.Headers.TryGetValue(ApiHeader.UserTypeId, out StringValues headerUserTypeId))
                return AuthenticateResult.Fail($"{ApiHeader.UserTypeId} is required");

            int? userTypeId = headerUserTypeId.ToString() switch
            {
                nameof(AuthenticationUserType.Guest) => (int)AuthenticationUserType.Guest,
                "0" => (int)AuthenticationUserType.Guest,
                nameof(AuthenticationUserType.Google) => (int)AuthenticationUserType.Google,
                "1" => (int)AuthenticationUserType.Google,
                nameof(AuthenticationUserType.Stocktwits) => (int)AuthenticationUserType.Stocktwits,
                "2" => (int)AuthenticationUserType.Stocktwits,
                _ => null
            };

            if (userTypeId is null)
                return AuthenticateResult.Fail($"{ApiHeader.UserTypeId} is invalid");

            //Step 4 - Check if Guest
            if (userTypeId == (int)AuthenticationUserType.Guest)
                return GetGuestAuthenticateResult(timezone);

            //Step 5 - Token or Code/Page
            if (!Request.Headers.TryGetValue(ApiHeader.Token, out StringValues token) & (!Request.Headers.TryGetValue(ApiHeader.Code, out StringValues code) | !Request.Headers.TryGetValue(ApiHeader.Page, out StringValues page)))
                return AuthenticateResult.Fail($"{ApiHeader.Token} or {ApiHeader.Code}/{ApiHeader.Page} is required");
            else if (token != StringValues.Empty && code != StringValues.Empty)
                return AuthenticateResult.Fail($"{ApiHeader.Token} and {ApiHeader.Code} both cannot have a value");

            //Step 6 - Check if route is Authenticate
            if (Request.Path.StartsWithSegments(Options.AuthenticateUrlSegment))
            {
                try
                {
                    if (token != StringValues.Empty)
                        return GetUserSessionAuthenticateResult(await _userService.AuthenticateTokenAsync(userTypeId.Value, token, timezone), timezone);

                    if (code != StringValues.Empty)
                        return GetUserSessionAuthenticateResult(await _userService.AuthenticateCodeAsync(userTypeId.Value, code, page, timezone), timezone);
                }
                catch (System.Exception exception)
                {
                    return AuthenticateResult.Fail(exception.Message);
                }
            }


            //Step 7 - Validate
            if (code != StringValues.Empty || page != StringValues.Empty)
                return AuthenticateResult.Fail($"{nameof(code)} is not allowed in this controller");

            try
            {
                return GetUserSessionAuthenticateResult(_userService.GetSession(userTypeId.Value, token), timezone);
            }
            catch (System.Exception exception)
            {
                return AuthenticateResult.Fail(exception.Message);
            }
        }

        private AuthenticateResult GetSwaggerAuthenticateResult()
        {
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(nameof(AuthenticationResult.IsAuthenticated), bool.TrueString)
            })), Scheme.Name));
        }

        private AuthenticateResult GetGuestAuthenticateResult(DateTimeZone timezone)
        {
            Request.HttpContext.Items.TryAdd(nameof(DateTimeZone), timezone);

            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(nameof(AuthenticationResult.IsAuthenticated), bool.TrueString)
            })), Scheme.Name));
        }

        private AuthenticateResult GetUserSessionAuthenticateResult(UserSessionContract userSession, DateTimeZone timezone)
        {
            Request.HttpContext.Items.TryAdd(nameof(DateTimeZone), timezone);

            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(nameof(AuthenticationResult.IsAuthenticated), bool.TrueString),
                new Claim(nameof(AuthenticationResult.UserSessionId), userSession.UserSessionId.ToString()),
                new Claim(nameof(AuthenticationResult.UserId), userSession.User.UserId.ToString()),
                new Claim(nameof(AuthenticationResult.UserTypeId), userSession.User.Type.TypeId.ToString())
            })), Scheme.Name));
        }
    }
}