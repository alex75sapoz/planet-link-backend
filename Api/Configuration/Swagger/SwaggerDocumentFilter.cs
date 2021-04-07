using Api.Configuration.Authentication;
using Api.Controller;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NodaTime;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Api.Configuration.Swagger
{
    internal class SwaggerDocumentFilter : IDocumentFilter
    {
        public SwaggerDocumentFilter(AuthenticationConfiguration authenticationConfiguration) =>
            _authenticationConfiguration = authenticationConfiguration;

        private readonly AuthenticationConfiguration _authenticationConfiguration;

        public void Apply(OpenApiDocument swaggerDocucment, DocumentFilterContext context)
        {
            if (context.SchemaRepository.TryLookupByType(typeof(AuthenticationUserType), out OpenApiSchema userTypeSchema))
            {
                var userTypeParameter = new OpenApiParameter()
                {
                    Name = ApiHeader.UserTypeId,
                    Required = true,
                    Schema = userTypeSchema,
                    AllowEmptyValue = false,
                    In = ParameterLocation.Header
                };

                var timezoneParameter = new OpenApiParameter()
                {
                    Name = ApiHeader.TimezoneId,
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = nameof(String).ToLower(),
                        Nullable = false,
                        Default = new OpenApiString(DateTimeZoneProviders.Tzdb.GetSystemDefault().Id)
                    },
                    In = ParameterLocation.Header
                };

                var tokenParameter = new OpenApiParameter()
                {
                    Name = ApiHeader.Token,
                    Required = false,
                    Schema = new OpenApiSchema()
                    {
                        Type = nameof(String).ToLower(),
                        Nullable = true
                    },
                    In = ParameterLocation.Header
                };

                var codeParameter = new OpenApiParameter()
                {
                    Name = ApiHeader.Code,
                    Required = false,
                    Schema = new OpenApiSchema()
                    {
                        Type = nameof(String).ToLower(),
                        Nullable = true
                    },
                    In = ParameterLocation.Header
                };

                var pageParameter = new OpenApiParameter()
                {
                    Name = ApiHeader.Page,
                    Required = false,
                    Schema = new OpenApiSchema()
                    {
                        Type = nameof(String).ToLower(),
                        Nullable = true
                    },
                    In = ParameterLocation.Header
                };

                foreach (var path in swaggerDocucment.Paths)
                {
                    path.Value.Parameters.Add(userTypeParameter);

                    path.Value.Parameters.Add(timezoneParameter);

                    path.Value.Parameters.Add(tokenParameter);

                    if (path.Key.StartsWith(_authenticationConfiguration.AuthenticateUrlSegment))
                    {
                        path.Value.Parameters.Add(codeParameter);

                        path.Value.Parameters.Add(pageParameter);
                    }
                }
            }
        }
    }
}