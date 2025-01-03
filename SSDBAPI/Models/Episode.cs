namespace SSDBAPI.Models
{
    public class Episode
    {
        /// <summary>
        ///     The date of this episode.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     The name of this episode.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The short name for this episode.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     The format of this episode (ex. Original, Livestream).
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        ///     Whether requests are open for this episode.
        /// </summary>
        public bool RequestsOpen { get; set; }

        /// <summary>
        ///     Whether to check the catalog to see if a song has been done before.
        ///     Values: Yes, No, Case-by-case
        /// </summary>
        public string CatalogChecksEnabled { get; set; }

        /// <summary>
        ///     Whether to check the banned list to see if an artist is banned.
        ///     Values: Yes, No, Case-by-case
        /// </summary>
        public string BannedListChecksEnabled { get; set; }
    }
}
