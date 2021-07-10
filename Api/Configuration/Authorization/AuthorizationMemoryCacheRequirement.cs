using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.User;
using Library.Weather;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    class AuthorizationLocationMemoryCacheRequirement : AuthorizationHandler<AuthorizationLocationMemoryCacheRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationLocationMemoryCacheRequirement requirement)
        {
            if (ILocationMemoryCache.IsReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    class AuthorizationProgrammingMemoryCacheRequirement : AuthorizationHandler<AuthorizationProgrammingMemoryCacheRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationProgrammingMemoryCacheRequirement requirement)
        {
            if (IProgrammingMemoryCache.IsReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    class AuthorizationStockMarketMemoryCacheRequirement : AuthorizationHandler<AuthorizationStockMarketMemoryCacheRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationStockMarketMemoryCacheRequirement requirement)
        {
            if (IStockMarketMemoryCache.IsReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    class AuthorizationUserMemoryCacheRequirement : AuthorizationHandler<AuthorizationUserMemoryCacheRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationUserMemoryCacheRequirement requirement)
        {
            if (IUserMemoryCache.IsReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    class AuthorizationWeatherMemoryCacheRequirement : AuthorizationHandler<AuthorizationWeatherMemoryCacheRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationWeatherMemoryCacheRequirement requirement)
        {
            if (IWeatherMemoryCache.IsReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}