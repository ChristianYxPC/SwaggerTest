using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwaggerTest.Enums;
using SwaggerTest.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SwaggerTest.Controllers
{
    [SwaggerTag("Test Controller")]
    [ApiVersion("1")]
    [ApiController]
    [Route("v{version:apiVersion}/Test")]
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

        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("v1.0")]
        public IEnumerable<WeatherForecast> GetWeatherForecastV1()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [ApiVersion("2")]
        [SwaggerOperation(Summary = "GetWeatherForecast Swagger Annotation", Description = "this is only a sample description")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<WeatherForecast>), Description = "Success")]
        [HttpGet("SwaggerAnnot")]
        public IEnumerable<WeatherForecast> GetUsingSwaggerAnnot()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [SwaggerOperation(Summary = "Test Phonenumber Mapper", Description = "Test PhoneNumber using swagger Type Mapper")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PhoneNumber), Description = "Success")]
        [HttpPost("PhoneNumber")]
        public PhoneNumber ShowPhonenumber([FromBody] PhoneNumber phoneNumber)
        {
            return phoneNumber;
        }

        [Authorize]
        [SwaggerOperation(Summary = "With Authorize Attribute", Description = "With Authorize Attribute")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(GenderEnum), Description = "Success")]
        [HttpPost("Authorize")]
        public IActionResult GetAuthorizeSample([FromQuery, SwaggerParameter("Gender Param")] GenderEnum gender)
        {
            return Ok(gender);
        }

        [SwaggerOperation(Summary = "Test Enum", Description = "Test Gender Enum")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(GenderEnum), Description = "Success")]
        [HttpPost("Gender")]
        public GenderEnum GetGender([FromQuery] GenderEnum gender)
        {
            return gender;
        }

        #region TestRequest

        [AllowAnonymous]
        [HttpPost("/FromForm/List/Item")]
        public IActionResult TestItemRequest(
            [FromForm] Item requests)
        //[FromForm] List<Item> requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(requests);
        }

        [AllowAnonymous]
        [HttpPost("/FromForm/List")]
        public IActionResult TestRequest(
            //[FromForm] Item requests)
            [FromForm] List<Item> requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(requests);
        }

        #endregion
    }
}