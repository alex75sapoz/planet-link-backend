using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration.Authorization
{
    internal static class AuthorizationExtension
    {
        public static void AddApiAuthorization(this IServiceCollection services) =>
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(AuthorizationDefaultRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationAnyRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationAnyRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationGoogleRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationGoogleRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationStocktwitsRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationStocktwitsRequirement()
                ));

                options.FallbackPolicy = options.GetPolicy(nameof(AuthorizationDefaultRequirement));
            });
    }
}