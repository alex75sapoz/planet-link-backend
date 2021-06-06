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
                options.AddPolicy(nameof(AuthorizationGoogleRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationGoogleRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationStocktwitsRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationStocktwitsRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationMemoryCacheLocationRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationMemoryCacheLocationRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationMemoryCacheProgrammingRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationMemoryCacheProgrammingRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationMemoryCacheStockMarketRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationMemoryCacheStockMarketRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationMemoryCacheUserRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationMemoryCacheUserRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationMemoryCacheWeatherRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationMemoryCacheWeatherRequirement()
                ));

                options.FallbackPolicy = options.GetPolicy(nameof(AuthorizationDefaultRequirement));
            });
    }
}