using Library.Error;
using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.User;
using Library.Weather;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NodaTime;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controller
{
    public class ApplicationController : ApiController
    {
        public ApplicationController(IWebHostEnvironment environment) =>
            _environment = environment;

        private readonly IWebHostEnvironment _environment;

        [HttpGet("Status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatusAsync() =>
            Ok(await Task.FromResult(new
            {
                IsProduction = _environment.IsProduction(),
                Server = new
                {
                    Time = DateTimeOffset.Now,
                    TimezoneId = DateTimeZoneProviders.Tzdb.GetSystemDefault().Id,
                    TotalMemoryAllocated = $"{GC.GetTotalMemory(false) / 1000000} MB"
                },
                Library = new
                {
                    Error = IErrorStartup.GetStatus(),
                    Location = ILocationStartup.GetStatus(),
                    Programming = IProgrammingStartup.GetStatus(),
                    StockMarket = IStockMarketStartup.GetStatus(),
                    User = IUserStartup.GetStatus(),
                    Weather = IWeatherStartup.GetStatus()
                },
                Jobs = Library.Base.ILibraryMemoryCache.Jobs.Select(job => new
                {
                    Id = job.Key,
                    Delay = job.Value.Delay.ToString(),
                    Interval = job.Value.Interval.ToString(),
                    job.Value.IsDependentOnCache,
                    job.Value.State,
                    job.Value.NextStartOn
                })
            }));
    }
}