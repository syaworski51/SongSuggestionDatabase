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
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "Short Name")]
        [MaxLength(20)]
        public string ShortName { get; set; }

        [Display(Name = "Format")]
        [MaxLength(10)]
        public string Format { get; set; }

        [Display(Name = "Requests open?")]
        public bool RequestsOpen { get; set; }

        [Display(Name = "Checking catalog?")]
        [MaxLength(12)]
        public string CatalogChecksEnabled { get; set; }

        [Display(Name = "Checking banned list?")]
        [MaxLength(12)]
        public string BannedListChecksEnabled { get; set; }
    }
}
