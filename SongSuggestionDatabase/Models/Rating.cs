using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongSuggestionDatabase.Models
{
    [Table("Ratings")]
    public class Rating
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Symbol")]
        public char Symbol { get; set; }

        [Display(Name = "Name")]
        [MaxLength(10)]
        public string Name { get; set; }

        [Display(Name = "Index")]
        public int Index { get; set; }

        [Display(Name = "Description")]
        [MaxLength(150)]
        public string Description { get; set; }

        [Display(Name = "Icon Path")]
        public string? IconPath { get; set; }
    }
}
