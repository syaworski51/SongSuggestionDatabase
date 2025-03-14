using System.ComponentModel.DataAnnotations;

namespace SSDBAPI.Models
{
    public class Currency
    {
        /// <summary>
        ///     The ISO 4217 code for this currency.
        /// </summary>
        [MaxLength(5)]
        public string Code { get; set; }

        /// <summary>
        ///     The symbol for this currency.
        /// </summary>
        [MaxLength(5)]
        public string Symbol { get; set; }

        /// <summary>
        ///     The name of this currency.
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
