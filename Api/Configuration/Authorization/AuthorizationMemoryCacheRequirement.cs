using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.User;
using Library.Weather;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    internal class AuthorizationMemoryCacheLocationRequirement : AuthorizationHandler<AuthorizationMemoryCacheLocationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationMemoryCacheLocationRequirement requirement)
        {
            if (ILocationStartup.IsMemoryCacheReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    internal class AuthorizationMemoryCacheProgrammingRequirement : AuthorizationHandler<AuthorizationMemoryCacheProgrammingRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationMemoryCacheProgrammingRequirement requirement)
        {
            if (IProgrammingStartup.IsMemoryCacheReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    internal class AuthorizationMemoryCacheStockMarketRequirement : AuthorizationHandler<AuthorizationMemoryCacheStockMarketRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationMemoryCacheStockMarketRequirement requirement)
        {
            if (IStockMarketStartup.IsMemoryCacheReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    internal class AuthorizationMemoryCacheUserRequirement : AuthorizationHandler<AuthorizationMemoryCacheUserRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationMemoryCacheUserRequirement requirement)
        {
            if (IUserStartup.IsMemoryCacheReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    internal class AuthorizationMemoryCacheWeatherRequirement : AuthorizationHandler<AuthorizationMemoryCacheWeatherRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationMemoryCacheWeatherRequirement requirement)
        {
            if (IWeatherStartup.IsMemoryCacheReady)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}