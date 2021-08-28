using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration.Authorization
{
    static class AuthorizationExtension
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
                options.AddPolicy(nameof(AuthorizationUserAdministratorRequirement), policy => policy.AddRequirements(
                    new AuthorizationDefaultRequirement(),
                    new AuthorizationUserAdministratorRequirement()
                ));

                options.FallbackPolicy = options.GetPolicy(nameof(AuthorizationDefaultRequirement));
            });
    }
}