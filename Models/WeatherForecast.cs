using Swashbuckle.AspNetCore.Annotations;

namespace SwaggerTest.Models
{
    public class WeatherForecast
    {
        [SwaggerSchema("Date of Weather Forecast ", Format = "date")]
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}