using Library.Programming;
using Library.Programming.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controller
{
    public class ProgrammingController : ApiController<IProgrammingService>
    {
        public ProgrammingController(IProgrammingService service) : base(service) { }

        [HttpGet("Configuration"), ProducesResponseType(typeof(ProgrammingConfigurationContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetConfigurationAsync() =>
            Ok(await Task.FromResult(_service.GetConfiguration()));

        [HttpGet("Project/Search"), ProducesResponseType(typeof(List<ProgrammingProjectContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59)]
        public async Task<IActionResult> SearchProjectsAsync([FromQuery] List<int> projectTypeIds, [FromQuery] List<int> languageIds, [FromQuery] List<int> jobIds, [FromQuery] List<int> technologyStackIds) =>
            Ok(await Task.FromResult(_service.SearchProjects(projectTypeIds, languageIds, jobIds, technologyStackIds)));
    }
}