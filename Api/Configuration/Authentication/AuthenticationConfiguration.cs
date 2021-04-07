using Microsoft.AspNetCore.Authentication;

namespace Api.Configuration.Authentication
{
    internal class AuthenticationConfiguration
    {
        public string Scheme { get; set; }
        public string AuthenticateUrlSegment { get; set; }
    }

    internal class AuthenticationScheme : AuthenticationSchemeOptions
    {
        public string SwaggerUrlSegment { get; set; }
        public string AuthenticateUrlSegment { get; set; }
        public bool IsDevelopment { get; set; }
    }
}