using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
/*
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }*/

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            var path = Environment.GetEnvironmentVariable("BusinessContract_Path");
            if (path == null)
            {
                Environment.SetEnvironmentVariable("BusinessContract_Path", "C:\\Contracts\\Business-Project");
                path = Environment.GetEnvironmentVariable("BusinessContract_Path");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            //Initialize PDF writer
            PdfWriter writer = new PdfWriter(path + "hello_world.pdf");
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(writer);
            // Initialize document
            Document document = new Document(pdf);
            //Add paragraph to the document
            document.Add(new Paragraph("Hello World!"));
            //Close document
            document.Close();
            return Ok(path);
        }
    }
}
