using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using var mutex = new Mutex(false, "MutexBug");
        var hasHandle = false;
        try
        {
            try
            {
                hasHandle = mutex.WaitOne(new TimeSpan(0, 0, 0, 5), false);
            }
            catch (AbandonedMutexException)
            {
                // Log the fact that the mutex was abandoned in another process,
                // it will still get acquired
                hasHandle = true;
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
        finally
        {
            if (hasHandle)
                mutex.ReleaseMutex();
        }
    }
}
