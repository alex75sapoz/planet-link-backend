using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    class AuthorizationUserAdministratorRequirement : AuthorizationHandler<AuthorizationUserAdministratorRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationUserAdministratorRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.IsAdministrator)}") == bool.TrueString)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}