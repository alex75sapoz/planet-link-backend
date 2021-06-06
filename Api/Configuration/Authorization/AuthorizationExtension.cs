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
                options.AddPolicy(nameof(AuthorizationUserTypeAnyRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationUserTypeAnyRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationUserTypeGoogleRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationUserTypeGoogleRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationUserTypeStocktwitsRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationUserTypeStocktwitsRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationLocationMemoryCacheRequirement), policy => policy.AddRequirements(
                    new AuthorizationLocationMemoryCacheRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationProgrammingMemoryCacheRequirement), policy => policy.AddRequirements(
                    new AuthorizationProgrammingMemoryCacheRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationStockMarketMemoryCacheRequirement), policy => policy.AddRequirements(
                    new AuthorizationStockMarketMemoryCacheRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationUserMemoryCacheRequirement), policy => policy.AddRequirements(
                    new AuthorizationUserMemoryCacheRequirement()
                ));
                options.AddPolicy(nameof(AuthorizationWeatherMemoryCacheRequirement), policy => policy.AddRequirements(
                    new AuthorizationWeatherMemoryCacheRequirement()
                ));

                options.FallbackPolicy = options.GetPolicy(nameof(AuthorizationDefaultRequirement));
            });
    }
}