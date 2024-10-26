using Microsoft.AspNetCore.Mvc;
using CsharpBackend.Models;

namespace CsharpBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageProcessingController : ControllerBase
    {


        private readonly ILogger<ImageProcessingController> _logger;

        public ImageProcessingController(ILogger<ImageProcessingController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Index")]
        public string Index()
        {
            return "This is my Core Sample Images application...";
        }

/*        [HttpGet(Name = "GetCoreSampleImage")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }*/
    }
}
