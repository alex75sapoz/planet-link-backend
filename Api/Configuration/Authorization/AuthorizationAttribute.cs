using Library.User.Enum;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Api.Configuration.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute() : base(nameof(AuthorizationAnyRequirement)) { }
        public AuthorizationAttribute(int userTypeId) : base(userTypeId switch
        {
            (int)UserType.Google => nameof(AuthorizationGoogleRequirement),
            (int)UserType.Stocktwits => nameof(AuthorizationStocktwitsRequirement),
            _ => throw new System.Exception($"{nameof(userTypeId)} is invalid in {nameof(AuthorizationAttribute)}")
        })
        { }
    }
}