using Api.Configuration.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    internal class AuthorizationAnyRequirement : AuthorizationHandler<AuthorizationAnyRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationAnyRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.UserSessionId)}") is not null)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}