using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SSDBAPI.Data;
using SSDBAPI.Models;

namespace SSDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannedListController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<BannedArtist> _collection;

        public BannedListController(MongoDbContext context)
        {
            _context = context;
            _collection = _context.GetCollection<BannedArtist>("bannedList");
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? name, string order = "asc")
        {
            var bannedList = _collection.Find(_ => true);
            bannedList = (order == "asc") ?
                bannedList.SortBy(a => a.Name) :
                bannedList.SortByDescending(a => a.Name);

            if (name != null)
                bannedList = _collection.Find(a => a.Name == name);

            return Ok(await bannedList.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BannedArtist bannedArtist)
        {
            await _collection.InsertOneAsync(bannedArtist);
            return Ok(bannedArtist);
        }

        [HttpPut]
        public async Task<IActionResult> Put(ObjectId id, [FromBody] BannedArtist updatedBannedArtist)
        {
            var artist = _collection.Find(a => a.Id == id).First();
            if (artist == null)
                return NotFound("The artist could not be found.");

            var update = await _collection.ReplaceOneAsync(a => a.Id == artist.Id, updatedBannedArtist);
            if (update.ModifiedCount == 0)
                return BadRequest($"There was an error updating {artist.Name} in the banned list.");

            return Ok(updatedBannedArtist);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(ObjectId id, BannedArtist bannedArtist)
        {
            var artist = await _collection.FindAsync(a => a.Id == id);
            if (artist == null)
                return NotFound($"{bannedArtist} could not be found.");

            var delete = _collection.DeleteOne(a => a.Id == id);
            if (delete.DeletedCount == 0)
                return BadRequest($"There was an error deleting {bannedArtist.Name} from the database.");

            return Ok();
        }
    }
}
