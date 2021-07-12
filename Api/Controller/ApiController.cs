using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace Api.Controller
{
    [Route("[controller]"), Produces("application/json"), ApiController]
    public abstract class ApiController : ControllerBase
    {
        private AuthenticationResult? _authenticationResult;

        protected AuthenticationResult AuthenticationResult =>
            _authenticationResult is null
                ? (_authenticationResult = new AuthenticationResult(User))
                : _authenticationResult;
        protected int? UserSessionId => AuthenticationResult.UserSessionId;
        protected int? UserId => AuthenticationResult.UserId;
        protected int? UserTypeId => AuthenticationResult.UserTypeId;
        protected DateTimeZone Timezone => AuthenticationResult.Timezone;
    }

    public abstract class ApiController<TService> : ApiController
    {
        protected ApiController(TService service) =>
            _service = service;

        protected readonly TService _service;
    }

    class ApiHeader
    {
        public const string Token = "api-token";

        public const string Code = "api-code";

        public const string UserTypeId = "api-userTypeId";

        public const string Subdomain = "api-subdomain";

        public const string Page = "api-page";

        public const string TimezoneId = "api-timezoneId";
    }
}