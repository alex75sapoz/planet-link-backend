global using Api.Configuration.Authentication;
global using Api.Configuration.Authorization;
global using Api.Configuration.Compression;
global using Api.Configuration.Controller;
global using Api.Configuration.CrossOriginRequest;
global using Api.Configuration.Database;
global using Api.Configuration.Endpoint;
global using Api.Configuration.Exception;
global using Api.Configuration.Library;
global using Api.Configuration.MemoryCache;
global using Api.Configuration.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;

            configuration = configuration.GetSection(nameof(Api));
            _databaseConfiguration = configuration.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>();
            _crossOriginRequestConfiguration = configuration.GetSection(nameof(CrossOriginRequestConfiguration)).Get<CrossOriginRequestConfiguration>();
            _endpointConfiguration = configuration.GetSection(nameof(EndpointConfiguration)).Get<EndpointConfiguration>();
            _authenticationConfiguration = configuration.GetSection(nameof(AuthenticationConfiguration)).Get<AuthenticationConfiguration>();
            _swaggerConfiguration = configuration.GetSection(nameof(SwaggerConfiguration)).Get<SwaggerConfiguration>();
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        private readonly DatabaseConfiguration _databaseConfiguration;
        private readonly CrossOriginRequestConfiguration _crossOriginRequestConfiguration;
        private readonly EndpointConfiguration _endpointConfiguration;
        private readonly AuthenticationConfiguration _authenticationConfiguration;
        private readonly SwaggerConfiguration _swaggerConfiguration;

        public void ConfigureServices(IServiceCollection services)
        {
            //Controller
            services.AddApiControllers();

            //Compression
            services.AddApiCompression();

            //Library
            services.AddApiLibrary(_configuration, _databaseConfiguration);

            //Memory Cache
            services.AddApiMemoryCache();

            //Authentication
            services.AddApiAuthentication(_environment, _authenticationConfiguration, _swaggerConfiguration);

            //Authorization
            services.AddApiAuthorization();

            //Cross Origin Request
            services.AddApiCrossOriginRequest(_crossOriginRequestConfiguration);

            //Swagger
            services.AddApiSwagger(_swaggerConfiguration, _authenticationConfiguration);
        }

        public void Configure(IApplicationBuilder application)
        {
            //Exception
            application.UseException();

            //Compression
            application.UseResponseCompression();

            //Https
            application.UseHttpsRedirection();

            //Route
            application.UseRouting();

            //Cross Origin Request
            application.UseCrossOriginRequest(_crossOriginRequestConfiguration);

            //Authentication
            application.UseAuthentication();

            //Authorization
            application.UseAuthorization();

            //Strict Transport Security
            application.UseHsts();

            //Endpoint
            application.UseEndpoint(_endpointConfiguration);

            //Swagger
            application.UseSwagger(_swaggerConfiguration);
        }
    }
}