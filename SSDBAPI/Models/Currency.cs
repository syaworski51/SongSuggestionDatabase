namespace SSDBAPI.Models
{
    public class Currency
    {
        /// <summary>
        ///     The ISO 4217 code for this currency.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     The symbol for this currency.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     The name of this currency.
        /// </summary>
        public string Name { get; set; }
    }
}
