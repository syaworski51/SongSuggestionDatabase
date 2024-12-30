using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongSuggestionDatabase.Models
{
    [Table("CatalogSortOptions")]
    public class CatalogSortOption
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}
