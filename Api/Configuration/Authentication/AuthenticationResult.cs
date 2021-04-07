using NodaTime;
using System.Security.Claims;

namespace Api.Configuration.Authentication
{
    public class AuthenticationResult
    {
        public AuthenticationResult(ClaimsPrincipal user)
        {
            IsAuthenticated = bool.TryParse(user.FindFirstValue(nameof(IsAuthenticated)), out bool isAuthenticated) && isAuthenticated;

            UserSessionId = int.TryParse(user.FindFirstValue(nameof(UserSessionId)), out int userSessionId)
                ? userSessionId
                : null;

            UserId = int.TryParse(user.FindFirstValue(nameof(UserId)), out int userId)
                ? userId
                : null;

            UserTypeId = int.TryParse(user.FindFirstValue(nameof(UserTypeId)), out int userTypeId)
                ? userTypeId
                : null;

            Timezone = DateTimeZoneProviders.Tzdb[user.FindFirstValue(nameof(Timezone))];
        }

        public bool IsAuthenticated { get; set; }
        public int? UserSessionId { get; set; }
        public int? UserId { get; set; }
        public int? UserTypeId { get; set; }
        public DateTimeZone Timezone { get; set; }
    }
}