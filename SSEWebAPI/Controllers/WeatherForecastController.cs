using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SSEWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static int timeout = 6000;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[Authorize]
        [HttpGet("/sse/GetSSEWeatherForecast")]
        public async Task GetSSEAsync()
        {
            await HttpContext.SSEInitAsync();
            long i = 0;
            while (true)
            {
                i++;
                await Task.Delay(timeout);
                var data = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
                await HttpContext.SSESendEventAsync(
                        new SSEEvent(
                            $"name-{i}",
                            JsonConvert.SerializeObject(data),
                            $"id-{i}"));
            }
        }
    }
}