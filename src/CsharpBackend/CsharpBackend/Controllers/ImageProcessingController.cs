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

        [HttpGet(Name = "GetImage/{PathToImage}")]
        public string GetImage(string PathToImage)
        {
            return "This is my Core Sample Images application...";
        }


    }
}
