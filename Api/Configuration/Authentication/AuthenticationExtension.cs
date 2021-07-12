using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Configuration.Authentication
{
    static class AuthenticationExtension
    {
        public static void AddApiAuthentication(this IServiceCollection services, IWebHostEnvironment environment, AuthenticationConfiguration configuration, SwaggerConfiguration swaggerConfiguration) =>
            services.AddAuthentication(configuration.Scheme)
                    .AddScheme<AuthenticationScheme, AuthenticationHandler>(configuration.Scheme, options =>
                    {
                        options.SwaggerUrlSegment = swaggerConfiguration.Path;
                        options.AuthenticateUrlSegment = configuration.AuthenticateUrlSegment;
                        options.IsDevelopment = environment.IsDevelopment();
                    });
    }
}