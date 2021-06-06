using Api.Configuration.Authentication;
using Api.Configuration.Authorization;
using Library.User;
using Library.User.Contract;
using Library.User.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable IDE0060 // Remove unused parameter
namespace Api.Controller
{
    [Authorization(Requirement.MemoryCacheUser)]
    public class UserController : ApiController<IUserService>
    {
        public UserController(IUserService service) : base(service) { }

        [HttpGet("Authenticate"), ProducesResponseType(typeof(UserSessionContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token)]
        [Authorization(Requirement.UserTypeGoogle), Authorization(Requirement.UserTypeStocktwits)]
        public async Task<IActionResult> AuthenticateSessionAsync([Required, FromHeader(Name = ApiHeader.UserTypeId)] AuthenticationUserType userTypeId) =>
            Ok(await Task.FromResult(_service.GetSession(UserSessionId.Value)));

        [HttpPost("Revoke"), ProducesResponseType((int)HttpStatusCode.NoContent)]
        [Authorization(Requirement.UserTypeGoogle), Authorization(Requirement.UserTypeStocktwits)]
        public async Task RemoveSessionAsync() =>
            await _service.RevokeSessionAsync(UserSessionId.Value);

        [HttpGet("ConsentUrl"), ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token)]
        public async Task<IActionResult> GetConsentUrlAsync([Required] UserType userTypeId, [Required] string subdomain, [Required] string page) =>
            Ok(await Task.FromResult(_service.GetUserConsentUrl((int)userTypeId, subdomain, page)));

        [HttpGet("Stocktwits/Search"), ProducesResponseType(typeof(List<UserContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token)]
        public async Task<IActionResult> SearchUsersStocktwitsAsync([Required] string keyword) =>
            Ok(await Task.FromResult(_service.SearchUsers(keyword, (int)UserType.Stocktwits)));
    }
}