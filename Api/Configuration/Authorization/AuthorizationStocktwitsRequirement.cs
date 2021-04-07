using Api.Configuration.Authentication;
using Api.Library.User.Enum;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    internal class AuthorizationStocktwitsRequirement : AuthorizationHandler<AuthorizationStocktwitsRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationStocktwitsRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.UserTypeId)}") == $"{(int)UserType.Stocktwits}")
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}