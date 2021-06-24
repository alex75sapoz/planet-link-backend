using Microsoft.AspNetCore.Authentication;

namespace Api.Configuration.Authentication
{
    class AuthenticationConfiguration
    {
        public string Scheme { get; set; } = default!;
        public string AuthenticateUrlSegment { get; set; } = default!;
    }

    class AuthenticationScheme : AuthenticationSchemeOptions
    {
        public string SwaggerUrlSegment { get; set; } = default!;
        public string AuthenticateUrlSegment { get; set; } = default!;
        public bool IsDevelopment { get; set; }
    }
}