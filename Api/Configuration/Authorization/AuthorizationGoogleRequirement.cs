﻿using Api.Configuration.Authentication;
using Library.User.Enum;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Configuration.Authorization
{
    internal class AuthorizationGoogleRequirement : AuthorizationHandler<AuthorizationGoogleRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationGoogleRequirement requirement)
        {
            if (context.User.FindFirstValue($"{nameof(AuthenticationResult.UserTypeId)}") == $"{(int)UserType.Google}")
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}