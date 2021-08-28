using Library.Account;
using Library.Account.Contract;
using Library.Account.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controller
{
    public class AccountController : ApiController<IAccountService>
    {
        public AccountController(IAccountService service) : base(service) { }

        [HttpGet("User/Authenticate"), ProducesResponseType(typeof(AccountUserSessionContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token)]
        [Authorization(Requirement.UserTypeAny)]
#pragma warning disable IDE0060 // Remove unused parameter
        public async Task<IActionResult> AuthenticateSessionAsync([Required, FromHeader(Name = ApiHeader.UserTypeId)] AuthenticationUserType userTypeId, [FromHeader(Name = ApiHeader.Subdomain)] string? subdomain) =>
#pragma warning restore IDE0060 // Remove unused parameter
            Ok(await Task.FromResult(IAccountMemoryCache.GetUserSession(UserSessionId!.Value)));

        [HttpPost("User/Revoke"), ProducesResponseType((int)HttpStatusCode.NoContent)]
        [Authorization(Requirement.UserTypeAny)]
        public async Task RemoveSessionAsync() =>
            await _service.RevokeUserSessionAsync(UserSessionId!.Value);

        [HttpGet("User/ConsentUrl"), ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token)]
        public async Task<IActionResult> GetConsentUrlAsync([Required] UserType userTypeId, [Required] string subdomain, [Required] string page) =>
            Ok(await Task.FromResult(_service.GetUserConsentUrl((int)userTypeId, subdomain, page)));

        [HttpGet("User/Stocktwits/Search"), ProducesResponseType(typeof(List<AccountUserContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token)]
        public async Task<IActionResult> SearchUsersStocktwitsAsync([Required] string keyword) =>
            Ok(await Task.FromResult(_service.SearchUsers(keyword, (int)UserType.Stocktwits)));
    }
}