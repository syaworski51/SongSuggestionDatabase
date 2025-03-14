using System.ComponentModel.DataAnnotations;

namespace SSDBAPI.Models
{
    public class Rating
    {
        public char Symbol { get; set; }

        [MaxLength(15)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string? IconPath { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }
    }
}
