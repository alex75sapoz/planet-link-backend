using Microsoft.AspNetCore.Authorization;
using System;

namespace Api.Configuration.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    internal class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(Requirement requirement) : base(requirement switch
        {
            Requirement.LocationMemoryCache => nameof(AuthorizationLocationMemoryCacheRequirement),
            Requirement.ProgrammingMemoryCache => nameof(AuthorizationProgrammingMemoryCacheRequirement),
            Requirement.StockMarketMemoryCache => nameof(AuthorizationStockMarketMemoryCacheRequirement),
            Requirement.UserTypeGoogle => nameof(AuthorizationUserTypeGoogleRequirement),
            Requirement.UserTypeStocktwits => nameof(AuthorizationUserTypeStocktwitsRequirement),
            Requirement.UserMemoryCache => nameof(AuthorizationUserMemoryCacheRequirement),
            Requirement.WeatherMemoryCache => nameof(AuthorizationWeatherMemoryCacheRequirement),
            _ => throw new System.Exception($"{requirement} is invalid in {nameof(AuthorizationAttribute)}")
        })
        { }
    }
}