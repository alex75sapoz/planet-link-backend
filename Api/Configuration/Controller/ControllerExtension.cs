using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Api.Configuration.Controller
{
    static class ControllerExtension
    {
        public static void AddApiControllers(this IServiceCollection services) =>
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    })
                    .ConfigureApiBehaviorOptions(options =>
                        options.InvalidModelStateResponseFactory = context =>
                            new BadRequestObjectResult(string.Join(", ", context.ModelState.Keys.Distinct().Select(key => $"{key} is {context.ModelState[key]?.ValidationState.ToString().ToLower()}")))
                    );
    }
}