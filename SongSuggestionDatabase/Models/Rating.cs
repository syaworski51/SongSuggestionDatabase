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
        public string Symbol { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Index")]
        public int Index { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Icon Path")]
        public string? IconPath { get; set; }
    }
}
