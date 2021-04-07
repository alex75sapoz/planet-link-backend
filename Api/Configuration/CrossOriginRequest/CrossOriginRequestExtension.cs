using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration.CrossOriginRequest
{
    internal static class CrossOriginRequestExtension
    {
        public static void AddApiCrossOriginRequest(this IServiceCollection services, CrossOriginRequestConfiguration configuration) =>
            services.AddCors(options => options.AddPolicy(configuration.Name, builder => builder.WithOrigins(configuration.Origins).WithHeaders(configuration.Headers).WithMethods(configuration.Methods)));

        public static void UseCrossOriginRequest(this IApplicationBuilder application, CrossOriginRequestConfiguration configuration) =>
            application.UseCors(configuration.Name);
    }
}