using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SSDBAPI.Models
{
    public class Episode
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        
        /// <summary>
        ///     The date of this episode.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     The name of this episode.
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        ///     The short name for this episode (i.e. "LSSF 2025-03-14")
        /// </summary>
        [MaxLength(20)]
        public string ShortName { get; set; }

        /// <summary>
        ///     The format of this episode (ex. Original, Livestream).
        /// </summary>
        [MaxLength(10)]
        public string Format { get; set; }

        /// <summary>
        ///     Whether requests are open for this episode.
        /// </summary>
        public bool RequestsOpen { get; set; }

        /// <summary>
        ///     List of all the songs featured in this episode.
        /// </summary>
        public List<Song> Songs { get; set; }

        /// <summary>
        ///     Whether to check the catalog to see if a song has been done before.
        ///     Values: Yes, No, Case-by-case
        /// </summary>
        [MaxLength(12)]
        public string CatalogChecksEnabled { get; set; }

        /// <summary>
        ///     Whether to check the banned list to see if an artist is banned.
        ///     Values: Yes, No, Case-by-case
        /// </summary>
        [MaxLength(12)]
        public string BannedListChecksEnabled { get; set; }
    }
}
