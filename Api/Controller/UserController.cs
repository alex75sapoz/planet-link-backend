using Api.Configuration.Authentication;
using Api.Configuration.Authorization;
using Api.Library.User;
using Api.Library.User.Contract;
using Api.Library.User.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable IDE0060 // Remove unused parameter
namespace Api.Controller
{
    public class UserController : ApiController<IUserService>
    {
        public UserController(IUserService service) : base(service) { }

        [HttpGet("Stocktwits/Search"), ResponseCache(Duration = 59), ProducesResponseType(typeof(List<UserContract>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchUsersStocktwitsAsync([Required] string keyword) =>
            Ok(await Task.FromResult(_service.SearchUsers(keyword, (int)UserType.Stocktwits)));

        [HttpGet("Authenticate"), Authorization, ResponseCache(Duration = 299, VaryByHeader = ApiHeader.UserTypeId + "," + ApiHeader.Token), ProducesResponseType(typeof(UserSessionContract), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AuthenticateSessionAsync([Required, FromHeader(Name = ApiHeader.UserTypeId)] AuthenticationUserType userTypeId)
        {
            var userSession = _service.GetSession(UserSessionId.Value);

            var cacheControlValues = Response.Headers.GetCommaSeparatedValues(HeaderNames.CacheControl);
            cacheControlValues[Array.FindIndex(cacheControlValues, cacheControlValue => cacheControlValue.StartsWith(CacheControlHeaderValue.MaxAgeString))] = $"{CacheControlHeaderValue.MaxAgeString}={(int)(userSession.TokenExpiresOn.AddMinutes(-5) - DateTimeOffset.Now).TotalSeconds}";
            Response.Headers.SetCommaSeparatedValues(HeaderNames.CacheControl, cacheControlValues);

            return Ok(await Task.FromResult(userSession));
        }

        [HttpPost("Revoke"), Authorization, ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task RemoveSessionAsync()
        {
            if (UserTypeId == (int)UserType.Stocktwits)
                return; //There is only one token per multiple devices, no need to revoke it

            await _service.RevokeSessionAsync(UserSessionId.Value);
        }

        [HttpGet("ConsentUrl"), ResponseCache(Duration = 299), ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetConsentUrlAsync([Required] UserType userTypeId, [Required] string page) =>
            Ok(await Task.FromResult(_service.GetUserConsentUrl((int)userTypeId, page)));
    }
}