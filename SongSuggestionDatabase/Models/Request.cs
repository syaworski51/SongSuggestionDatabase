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
        public string RequestedBy { get; set; }

        [ForeignKey(nameof(Currency))]
        public string? CurrencyCode { get; set; }

        [Display(Name = "Currency")]
        public Currency? Currency { get; set; }

        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }

        [Display(Name = "to USD")]
        public decimal? USDAmount { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Artist")]
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
        public string? DetailedRating { get; set; }

        [Display(Name = "Quote")]
        public string? Quote { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public bool IsInQueue => !IsPlaying && TimeCompleted == null;

        public bool IsComplete => !IsPlaying && TimeCompleted != null;
    }
}
