namespace SSDBAPI.Models
{
    public class Song
    {
        /// <summary>
        ///     The time this song was requested. Determines playing order if there is a song
        ///     that was requested with the same amount.
        /// </summary>
        public DateTime TimeRequested { get; set; }

        /// <summary>
        ///     The time that this song request was completed.
        /// </summary>
        public DateTime? TimeCompleted { get; set; }

        /// <summary>
        ///     The YouTube username of the person that requested this song.
        /// </summary>
        public string RequestedBy { get; set; }

        /// <summary>
        ///     The currency used to pay for this request.
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        ///     The amount of money paid in the specified currency to pay for this request.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///     The request amount converted to US Dollars.
        /// </summary>
        public decimal USDAmount { get; set; }

        /// <summary>
        ///     The title of the song.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The artist of the song.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     The episode this song was requested in.
        /// </summary>
        public Episode Episode { get; set; }

        /// <summary>
        ///     The rating given to this song by the host.
        ///     
        ///     +: Would actively seek out and listen to again.
        ///     =: Wouldn't mind listening to, but wouldn't actively seek out.
        ///     -: Would actively avoid listening to this song.
        /// </summary>
        public char? Rating { get; set; }

        /// <summary>
        ///     The rating of the song explained in more detail.
        /// </summary>
        public string? DetailedRating { get; set; }

        /// <summary>
        ///     Quotes from when this song was playing in the livestream.
        /// </summary>
        public string? Quote { get; set; }
    }
}
