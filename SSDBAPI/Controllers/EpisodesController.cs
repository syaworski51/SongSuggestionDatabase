using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SSDBAPI.Data;
using SSDBAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SSDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private const string ORIGINAL = "Original";
        private const string LIVESTREAM = "Livestream";
        
        private const string YES = "Yes";
        private const string NO = "No";
        private const string CASE_BY_CASE = "Case-by-case";

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Episode> _collection;

        public EpisodesController(MongoDbContext context)
        {
            _context = context;
            _collection = _context.GetCollection<Episode>("episodes");
        }
        
        
        // GET: api/<EpisodesController>
        [HttpGet]
        public async Task<IActionResult> Get(string order = "desc")
        {
            if (_collection == null)
                return NotFound();

            var episodes = _collection.Find(_ => true);
            episodes = (order == "asc") ?
                episodes.SortBy(s => s.Date) :
                episodes.SortByDescending(s => s.Date);

            return Ok(await episodes.ToListAsync());
        }

        // GET api/<EpisodesController>/5
        [HttpGet]
        public async Task<IActionResult> Get(DateTime date)
        {
            if (_collection == null)
                return NotFound();

            var episode = _collection.Find(e => e.Date == date);
            return Ok(episode.First());
        }

        // POST api/<EpisodesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Episode episode)
        {
            if (episode.Format != ORIGINAL && episode.Format != LIVESTREAM)
                return BadRequest($"{episode.Format} is not a valid episose format. Please try again.");

            if (episode.CatalogChecksEnabled != YES &&
                episode.CatalogChecksEnabled != NO &&
                episode.CatalogChecksEnabled != CASE_BY_CASE)
                return BadRequest($"{episode.CatalogChecksEnabled} is not a valid value for catalog checks. Please try again.");

            if (episode.BannedListChecksEnabled != YES &&
                episode.BannedListChecksEnabled != NO &&
                episode.BannedListChecksEnabled != CASE_BY_CASE)
                return BadRequest($"{episode.CatalogChecksEnabled} is not a valid value for checking the banned list. Please try again.");

            await _collection.InsertOneAsync(episode);
            return Ok(episode);
        }

        // PUT api/<EpisodesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(ObjectId id, [FromBody] Episode episode)
        {
            return NoContent();
        }

        /// <summary>
        ///     Add a song to the database.
        /// </summary>
        /// <param name="episodeId">The episode to add this song to.</param>
        /// <param name="song">The song to add to the database.</param>
        /// <returns>The song that was just added to the database.</returns>
        public async Task<IActionResult> AddSong(ObjectId episodeId, [FromBody] Song song)
        {
            // Search for the episode by its ID
            // If it cannot be found, return a NotFound() message
            var episode = _collection.Find(e => e.Id == episodeId).First();
            if (episode == null)
                return NotFound($"The episode with ID {episodeId.ToString()} could not be found.");

            // Search through the song collection to see if the requested song has been done before
            // If catalog checks are enabled AND the requested song has been done, return a BadRequest() message
            var songs = _context.GetCollection<Song>("songs");
            bool songHasBeenDoneBefore = songs.Find(s => s.Artist == song.Artist && s.Title == song.Title).Any();
            if (episode.CatalogChecksEnabled == YES && songHasBeenDoneBefore)
                return BadRequest($"{song.Title} by {song.Artist} has already been done before. Please pick another song.");

            // Search through the banned list to see if the requested artist is banned
            // If banned list checks are enabled AND the requested artist is banned, return a BadRequest() message
            var bannedList = _context.GetCollection<BannedArtist>("bannedList");
            bool artistIsBanned = bannedList.Find(a => a.Name == song.Artist).Any();
            if (episode.BannedListChecksEnabled == YES && artistIsBanned)
                return BadRequest($"{song.Artist} has been banned from the livestreams. Please pick another artist.");

            // If requests are closed, do not allow this request to go through
            if (!episode.RequestsOpen)
                return BadRequest("Requests have been closed for this episode.");

            
            // If we have arrived at this point, the song has been validated and can be added to the database
            episode.Songs.Add(song);
            songs.InsertOne(song);

            // Return the newly inserted song
            return Ok(song);
        }

        public async Task<IActionResult> UpdateSong(ObjectId episodeId, ObjectId songId, [FromBody] Song updatedSong)
        {
            var episode = _collection.Find(e => e.Id == episodeId).First();
            if (episode == null)
                return NotFound($"The episode with ID {episodeId} could not be found.");

            var songs = _context.GetCollection<Song>("songs");
            var song = songs.Find(s => s.Id == songId).First();
            if (song == null)
                return NotFound($"The song could not be found.");

            var updateSong = songs.ReplaceOne(s => s.Id == songId, updatedSong);
            if (updateSong.ModifiedCount == 0)
                return BadRequest($"There was an error in updating the song.");

            return Ok();
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(ObjectId id)
        {
            var episode = _collection.Find(e => e.Id == id).First();
            if (episode == null)
                return NotFound($"The episode with ID {id} could not be found.");

            _collection.DeleteOne(e => e.Id == id);
            return Ok();
        }
    }
}
