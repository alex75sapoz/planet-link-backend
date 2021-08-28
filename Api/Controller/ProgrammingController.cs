using Library.Programming;
using Library.Programming.Contract;
using Library.Programming.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> SearchProjectsAsync([Range(1, int.MaxValue)] int? projectTypeId, [Range(1, int.MaxValue)] int? languageId, [Range(1, int.MaxValue)] int? jobId, [Range(1, int.MaxValue)] int? technologyStackId) =>
            Ok(await Task.FromResult(_service.SearchProjects(projectTypeId, languageId, jobId, technologyStackId)));

        [HttpPost("Project"), ProducesResponseType(typeof(ProgrammingProjectContract), (int)HttpStatusCode.OK)]
        [Authorization(Requirement.UserTypeGoogle)]
        public async Task<IActionResult> CreateProjectAsync([Required] ProgrammingProjectCreateContract newProject) =>
            Ok(await _service.CreateProjectAsync(newProject, Timezone));

        [HttpPost("MemoryCache/Refresh")]
        public async Task MemoryCacheRefreshAsync(ProgrammingDictionary? dictionary = null, int? id = null) =>
            await _service.MemoryCacheRefreshAsync(dictionary, id);
    }
}