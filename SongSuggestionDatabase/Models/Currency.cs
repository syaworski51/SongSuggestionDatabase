using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongSuggestionDatabase.Models
{
    [Table("Currencies")]
    public class Currency
    {
        /// <summary>
        ///     ISO 4217 code for this currency (ex. CAD = Canadian dollar).
        /// </summary>
        [Key]
        [Display(Name = "Code")]
        [MaxLength(5)]
        public string Code { get; set; }
        
        /// <summary>
        ///     YouTube superchat symbol for this currency.
        /// </summary>
        [Display(Name = "Symbol")]
        [MaxLength(5)]
        public string Symbol { get; set; }

        /// <summary>
        ///     The name of this currency.
        /// </summary>
        [Display(Name = "Name")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
