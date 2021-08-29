using Library.Base;
using Library.Location;
using Library.Location.Contract;
using Library.Location.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controller
{
    public class LocationController : ApiController<ILocationService>
    {
        public LocationController(ILocationService service) : base(service) { }

        [HttpGet("Country"), ProducesResponseType(typeof(LocationCountryContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetCountryAsync([Required, Range(1, int.MaxValue)] int countryId) =>
            Ok(await Task.FromResult(ILocationMemoryCache.GetCountry(countryId)));

        [HttpGet("Country/Search"), ProducesResponseType(typeof(List<LocationCountryContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> SearchCountriesAsync([Required] string keyword) =>
            Ok(await Task.FromResult(_service.SearchCountries(keyword)));


        [HttpGet("City"), ProducesResponseType(typeof(LocationCityContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetCityAsync([Range(1, int.MaxValue)] int? cityId, [Range(-90, 90)] decimal? latitude, [Range(-180, 180)] decimal? longitude) =>
            cityId.HasValue
                ? Ok(await Task.FromResult(ILocationMemoryCache.GetCity(cityId.Value)))
                : latitude.HasValue && longitude.HasValue
                    ? Ok(await Task.FromResult(_service.GetCity((latitude.Value, longitude.Value))))
                    : throw new BadRequestException($"{nameof(cityId)} or {nameof(latitude)}/{nameof(longitude)} is required");

        [HttpGet("City/Search"), ProducesResponseType(typeof(List<LocationCityContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> SearchCitiesAsync([Required] string keyword) =>
            Ok(await Task.FromResult(_service.SearchCities(keyword)));

        [HttpPost("MemoryCache/Refresh")]
        [Authorization(Requirement.UserAdministrator)]
        public async Task MemoryCacheRefreshAsync(LocationDictionary? dictionary = null, int? id = null) =>
            await _service.MemoryCacheRefreshAsync(dictionary, id);
    }
}