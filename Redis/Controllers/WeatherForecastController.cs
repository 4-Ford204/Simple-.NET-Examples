using Microsoft.AspNetCore.Mvc;
using Redis.Attributes;
using Redis.Services;

namespace Redis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IResponseCacheService _responseCacheService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IResponseCacheService responseCacheService)
        {
            _logger = logger;
            _responseCacheService = responseCacheService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [Cache(1000)]
        public async Task<IActionResult> Get(string keyword = null, int pageIndex = 1, int pageSize = 5)
        {
            var result = Summaries
                .Select(x => new WeatherForecast() { Name = x })
                .Where(x => string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(result);
        }

        [HttpGet("DeleteWeatherForecast")]
        public async Task<IActionResult> Delete()
        {
            await _responseCacheService.RemoveCacheResponseAsync("/WeatherForecast");
            return Ok();
        }
    }
}
