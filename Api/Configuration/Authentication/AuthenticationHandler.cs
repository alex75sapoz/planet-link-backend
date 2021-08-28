using Api.Controller;
using Library.Account;
using Library.Account.Contract;
using Library.Application;
using Library.Application.Contract;
using Library.Base;
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
    class AuthenticationHandler : AuthenticationHandler<AuthenticationScheme>
    {
        public AuthenticationHandler(IOptionsMonitor<AuthenticationScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAccountService accountService, IApplicationService applicationService) : base(options, logger, encoder, clock)
        {
            _accountService = accountService;
            _applicationService = applicationService;
        }

        private readonly IAccountService _accountService;
        private readonly IApplicationService _applicationService;

        private StringValues _headerTimezoneId;
        private StringValues _headerUserTypeId;
        private StringValues _headerToken;
        private StringValues _headerCode;
        private StringValues _headerSubdomain;
        private StringValues _headerPage;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Swagger is publicly available
            if (Request.Path.StartsWithSegments(Options.SwaggerUrlSegment))
                return GetSwaggerAuthenticateResult();

            //Step 1 - TimezoneId
            if (!Request.Headers.TryGetValue(ApiHeader.TimezoneId, out _headerTimezoneId)) return await GetUnauthorizedResult($"{ApiHeader.TimezoneId} is required");
            var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(_headerTimezoneId);
            if (timezone is null) return await GetUnauthorizedResult($"{ApiHeader.TimezoneId} is invalid");

            //Step 2 - UserTypeId
            if (!Request.Headers.TryGetValue(ApiHeader.UserTypeId, out _headerUserTypeId)) return await GetUnauthorizedResult($"{ApiHeader.UserTypeId} is required");
            int? userTypeId = _headerUserTypeId.ToString() switch
            {
                nameof(AuthenticationUserType.Guest) => (int)AuthenticationUserType.Guest,
                "0" => (int)AuthenticationUserType.Guest,
                nameof(AuthenticationUserType.Google) => (int)AuthenticationUserType.Google,
                "1" => (int)AuthenticationUserType.Google,
                nameof(AuthenticationUserType.Stocktwits) => (int)AuthenticationUserType.Stocktwits,
                "2" => (int)AuthenticationUserType.Stocktwits,
                nameof(AuthenticationUserType.Fitbit) => (int)AuthenticationUserType.Fitbit,
                "3" => (int)AuthenticationUserType.Fitbit,
                _ => null
            };
            if (userTypeId is null) return await GetUnauthorizedResult($"{ApiHeader.UserTypeId} is invalid");

            //Step 3 - Check if Guest
            if (userTypeId == (int)AuthenticationUserType.Guest) return GetGuestAuthenticateResult(timezone);

            //Step 4 - Token or Code/Subdomain/Page
            if (!Request.Headers.TryGetValue(ApiHeader.Token, out _headerToken) & (!Request.Headers.TryGetValue(ApiHeader.Code, out _headerCode) | !Request.Headers.TryGetValue(ApiHeader.Subdomain, out _headerSubdomain) | !Request.Headers.TryGetValue(ApiHeader.Page, out _headerPage)))
                return await GetUnauthorizedResult($"{ApiHeader.Token} or {ApiHeader.Code}/{ApiHeader.Page} is required");
            else if (_headerToken != StringValues.Empty && _headerCode != StringValues.Empty)
                return await GetUnauthorizedResult($"{ApiHeader.Token} and {ApiHeader.Code} both cannot have a value");

            //Step 5 - Check if route is Authenticate
            if (Request.Path.StartsWithSegments(Options.AuthenticateUrlSegment))
            {
                try
                {
                    if (_headerToken != StringValues.Empty)
                        return GetUserSessionAuthenticateResult(await _accountService.AuthenticateUserTokenAsync(userTypeId.Value, _headerToken, timezone), timezone);

                    if (_headerCode != StringValues.Empty)
                        return GetUserSessionAuthenticateResult(await _accountService.AuthenticateUserCodeAsync(userTypeId.Value, _headerCode, _headerSubdomain, _headerPage, timezone), timezone);
                }
                catch (System.Exception exception)
                {
                    return await GetUnauthorizedResult(exception);
                }
            }

            //Step 6 - Validate
            if (_headerCode != StringValues.Empty || _headerPage != StringValues.Empty)
                return await GetUnauthorizedResult($"{nameof(ApiHeader.Code)} or {nameof(ApiHeader.Page)} is not allowed in this controller");

            try
            {
                return GetUserSessionAuthenticateResult(IAccountMemoryCache.GetUserSession(userTypeId.Value, _headerToken, isExpiredSessionValid: false), timezone);
            }
            catch (System.Exception exception)
            {
                return await GetUnauthorizedResult(exception);
            }
        }

        private AuthenticateResult GetSwaggerAuthenticateResult() =>
            AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(nameof(AuthenticationResult.IsAuthenticated), bool.TrueString)
            })), Scheme.Name));

        private AuthenticateResult GetGuestAuthenticateResult(DateTimeZone timezone) =>
            AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(nameof(AuthenticationResult.IsAuthenticated), bool.TrueString),
                new Claim(nameof(AuthenticationResult.Timezone), timezone.Id)
            })), Scheme.Name));

        private AuthenticateResult GetUserSessionAuthenticateResult(AccountUserSessionContract userSession, DateTimeZone timezone) =>
            AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(nameof(AuthenticationResult.IsAuthenticated), bool.TrueString),
                new Claim(nameof(AuthenticationResult.UserSessionId), userSession.UserSessionId.ToString()),
                new Claim(nameof(AuthenticationResult.UserId), userSession.UserId.ToString()),
                new Claim(nameof(AuthenticationResult.UserTypeId), userSession.User.UserTypeId.ToString()),
                new Claim(nameof(AuthenticationResult.IsAdministrator), userSession.User.IsAdministrator.ToString()),
                new Claim(nameof(AuthenticationResult.Timezone), timezone.Id)
            })), Scheme.Name));

        private async Task<AuthenticateResult> GetUnauthorizedResult(string message) =>
            await GetUnauthorizedResult(new UnauthorizedException(message));

        private async Task<AuthenticateResult> GetUnauthorizedResult(System.Exception exception)
        {
            await _applicationService.CreateErrorAuthenticationAsync(new ApplicationErrorAuthenticationCreateContract
            (
                method: Request.Method,
                path: Request.Path,
                query: Request.QueryString.ToString(),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            )
            {
                TimezoneId = _headerTimezoneId,
                UserTypeId = _headerUserTypeId,
                Token = _headerToken,
                Code = _headerCode,
                Subdomain = _headerSubdomain,
                Page = _headerPage
            });

            return AuthenticateResult.Fail(exception);
        }

    }
}