namespace SSDBAPI.Models
{
    public class BannedArtist
    {
        /// <summary>
        ///     The name of the banned artist.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Whether to ignore this entry in banned list checks.
        /// </summary>
        public bool IgnoreInChecks { get; set; }

        /// <summary>
        ///     Whether this artist is permanently banned from the livestreams.
        /// </summary>
        public bool IsPermanentlyBanned { get; set; }

        /// <summary>
        ///     Any comments left regarding this artist.
        /// </summary>
        public string? Comments { get; set; }
    }
}
