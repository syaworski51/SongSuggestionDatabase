using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongSuggestionDatabase.Models
{
    [Table("Episodes")]

    public class Episode
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Checking catalog?")]
        public string CatalogChecksEnabled { get; set; }

        [Display(Name = "Checking banned list?")]
        public string BannedListChecksEnabled { get; set; }
    }
}
