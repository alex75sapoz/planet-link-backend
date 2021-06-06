﻿using Api.Configuration.Authentication;
using Library.User.Enum;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    internal class AuthorizationUserTypeGoogleRequirement : AuthorizationHandler<AuthorizationUserTypeGoogleRequirement>, IAuthorizationRequirement
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

    internal class AuthorizationUserTypeStocktwitsRequirement : AuthorizationHandler<AuthorizationUserTypeStocktwitsRequirement>, IAuthorizationRequirement
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