using Microsoft.AspNetCore.Authorization;
using System;

namespace Api.Configuration.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    internal class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(Requirement requirement) : base(requirement switch
        {
            Requirement.UserTypeGoogle => nameof(AuthorizationGoogleRequirement),
            Requirement.UserTypeStocktwits => nameof(AuthorizationStocktwitsRequirement),
            Requirement.MemoryCacheLocation => nameof(AuthorizationMemoryCacheLocationRequirement),
            Requirement.MemoryCacheProgramming => nameof(AuthorizationMemoryCacheProgrammingRequirement),
            Requirement.MemoryCacheStockMarket => nameof(AuthorizationMemoryCacheStockMarketRequirement),
            Requirement.MemoryCacheUser => nameof(AuthorizationMemoryCacheUserRequirement),
            Requirement.MemoryCacheWeather => nameof(AuthorizationMemoryCacheWeatherRequirement),
            _ => throw new System.Exception($"{requirement} is invalid in {nameof(AuthorizationAttribute)}")
        })
        { }
    }
}