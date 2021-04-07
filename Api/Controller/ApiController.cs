using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System.Security.Claims;

namespace Api.Controller
{
    [Route("[controller]"), Produces("application/json"), ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected int? UserSessionId => int.TryParse(User.FindFirstValue(nameof(UserSessionId)), out int userSessionId)
            ? userSessionId
            : null;

        protected int? UserId => int.TryParse(User.FindFirstValue(nameof(UserId)), out int userId)
            ? userId
            : null;

        protected int? UserTypeId => int.TryParse(User.FindFirstValue(nameof(UserTypeId)), out int userTypeId)
            ? userTypeId
            : null;

        protected DateTimeZone Timezone => (DateTimeZone)HttpContext.Items[nameof(DateTimeZone)];
    }

    public abstract class ApiController<TService> : ApiController
    {
        protected ApiController(TService service) =>
            _service = service;

        protected readonly TService _service;
    }

    internal class ApiHeader
    {
        public const string Token = "api-token";

        public const string Code = "api-code";

        public const string UserTypeId = "api-userTypeId";

        public const string Page = "api-page";

        public const string TimezoneId = "api-timezoneId";
    }
}