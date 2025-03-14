using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongSuggestionDatabase.Models
{
    [Table("BannedList")]
    public class BannedArtist
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "Ignore?")]
        public bool IgnoreInChecks { get; set; }

        [Display(Name = "Permanently banned?")]
        public bool IsPermanentlyBanned { get; set; }

        [Display(Name = "Comments")]
        [MaxLength(200)]
        public string? Comments { get; set; }
    }
}
