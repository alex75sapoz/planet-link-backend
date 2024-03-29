﻿using Microsoft.AspNetCore.Builder;

namespace Api.Configuration.Endpoint
{
    static class EndpointExtension
    {
        public static void UseEndpoint(this IApplicationBuilder application, EndpointConfiguration configuration) =>
            application.UseEndpoints(endpoint => endpoint.MapControllerRoute(configuration.Name, configuration.Pattern));
    }
}