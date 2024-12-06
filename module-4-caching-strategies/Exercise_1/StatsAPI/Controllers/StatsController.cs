using Microsoft.AspNetCore.Mvc;
using StatsAPI.Models;
using StatsAPI.Services;

namespace StatsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IMockGenerationService _mockGenerationService;
        public StatsController(DatabaseService databaseService, IMockGenerationService mockGenerationService)
        {
            _databaseService = databaseService;
            _mockGenerationService = mockGenerationService;
        }

        [HttpPost]
        [Route("SendResult")]
        public async Task<IActionResult> PostGameResult([FromBody] Stats stats)
        {
            await _databaseService.AddStats(stats);
            return Ok();
        }

        [HttpGet]
        [Route("GetStats/{period}")]
        public async Task<IActionResult> GetStats(Period period)
        {
            var result = await _databaseService.ReadResult(period);
            return Ok(result);
        }

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> GenerateData([FromBody] int count)
        {
            var itemsToInsert = _mockGenerationService.CreateMockData(count);
            List<Task> tasks = new List<Task>();
            foreach (var item in itemsToInsert)
            {
                tasks.Add(_databaseService.AddStats(item));
            }
            await Task.WhenAll(tasks);
            return Ok();
        }
    }
}
