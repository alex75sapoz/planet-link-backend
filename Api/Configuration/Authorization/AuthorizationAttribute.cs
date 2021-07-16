using Microsoft.AspNetCore.Authorization;
using System;

namespace Api.Configuration.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(Requirement requirement) : base(requirement switch
        {
            Requirement.UserTypeAny => nameof(AuthorizationUserTypeAnyRequirement),
            Requirement.UserTypeGoogle => nameof(AuthorizationUserTypeGoogleRequirement),
            Requirement.UserTypeStocktwits => nameof(AuthorizationUserTypeStocktwitsRequirement),
            _ => throw new System.Exception($"{requirement} is invalid in {nameof(AuthorizationAttribute)}")
        })
        { }
    }
}