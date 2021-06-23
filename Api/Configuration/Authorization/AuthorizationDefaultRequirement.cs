using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    internal class AuthorizationDefaultRequirement : AuthorizationHandler<AuthorizationDefaultRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationDefaultRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.IsAuthenticated)}") == bool.TrueString)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}