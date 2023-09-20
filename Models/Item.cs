using static SwaggerTest.Controllers.WeatherForecastController;
using System.ComponentModel.DataAnnotations;
using SwaggerTest.Enums;
using System.ComponentModel;

namespace SwaggerTest.Models
{
    public class Item : NestedItem
    {
        public NestedItem NestedItem { get; set; }
        [Required]
        public List<NestedItem> ListNestedItems { get; set; }
        public IFormFileCollection? Images { get; set; }

        [Required]
        public List<int> Ints { get; set; }
    }
    public class NestedItem
    {
        [Required]
        public int Int { get; set; }
        [DefaultValue("Sample String Value")]
        public string String { get; set; }
        public double Double { get; set; }
        public bool Boolean { get; set; }
        public GenderEnum EnumValue { get; set; }
        private string Private { get; set; }
        public DateTime date { get; set; }
        public IFormFile Image { get; set; }
        public int? NInt { get; set; }
        public string? NString { get; set; }
        public double? NDouble { get; set; }
        public bool? NBoolean { get; set; }
        public GenderEnum? NEnumValue { get; set; }
        private string? NPrivate { get; set; }
        public DateTime? Ndate { get; set; }
        public IFormFile? NImage { get; set; }
    }

}
