using Api.Configuration.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Api.Configuration.Swagger
{
    internal static class SwaggerExtension
    {
        public static void AddApiSwagger(this IServiceCollection services, SwaggerConfiguration configuration, AuthenticationConfiguration authenticationConfiguration) =>
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString().Replace($"{nameof(Api)}.{nameof(Configuration)}.", string.Empty).Replace($"{nameof(Api)}.{nameof(Library)}.", string.Empty));
                options.DocumentFilter<SwaggerDocumentFilter>(authenticationConfiguration);
                options.SwaggerDoc("v1", new OpenApiInfo { Title = configuration.Title });
            });

        public static void UseSwagger(this IApplicationBuilder application, SwaggerConfiguration configuration) =>
            application.UseSwagger().UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{configuration.Path}/v1/swagger.json", configuration.Title);
                options.RoutePrefix = configuration.Path[1..];
                options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
            });
    }
}