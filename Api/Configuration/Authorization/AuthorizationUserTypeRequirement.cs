using Library.User.Enum;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    class AuthorizationUserTypeAnyRequirement : AuthorizationHandler<AuthorizationUserTypeAnyRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationUserTypeAnyRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.UserSessionId)}") is not null)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    class AuthorizationUserTypeGoogleRequirement : AuthorizationHandler<AuthorizationUserTypeGoogleRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationUserTypeGoogleRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.UserTypeId)}") == $"{(int)UserType.Google}")
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }

    class AuthorizationUserTypeStocktwitsRequirement : AuthorizationHandler<AuthorizationUserTypeStocktwitsRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationUserTypeStocktwitsRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.UserTypeId)}") == $"{(int)UserType.Stocktwits}")
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}