using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongSuggestionDatabase.Models
{
    [Table("Songs")]
    public class Request
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Time Requested")]
        public DateTime TimeRequested { get; set; }

        [Display(Name = "Time Completed")]
        public DateTime? TimeCompleted { get; set; }

        [Display(Name = "Requested By")]
        [MaxLength(30)]
        public string RequestedBy { get; set; }

        [ForeignKey(nameof(Currency))]
        [MaxLength(5)]
        public string CurrencyCode { get; set; }

        [Display(Name = "Currency")]
        public Currency Currency { get; set; }

        [Display(Name = "Amount")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? Amount { get; set; }

        [Display(Name = "to USD")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? USDAmount { get; set; }

        [Display(Name = "Title")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "Artist")]
        [MaxLength(50)]
        public string Artist { get; set; }

        [ForeignKey(nameof(Episode))]
        public Guid? EpisodeId { get; set; }
        
        [Display(Name = "Episode")]
        public Episode? Episode { get; set; }

        [Display(Name = "Playing?")]
        public bool IsPlaying { get; set; }

        [ForeignKey(nameof(Rating))]
        public Guid? RatingId { get; set; }

        [Display(Name = "Rating")]
        public Rating? Rating { get; set; }

        [Display(Name = "Detailed Rating")]
        [MaxLength(200)]
        public string? DetailedRating { get; set; }

        [Display(Name = "Quote")]
        [MaxLength(200)]
        public string? Quote { get; set; }

        [Display(Name = "Status")]
        [MaxLength(15)]
        public string Status { get; set; }

        public bool IsInQueue => !IsPlaying && TimeCompleted == null;

        public bool IsComplete => !IsPlaying && TimeCompleted != null;
    }
}
