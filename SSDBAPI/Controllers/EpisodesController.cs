using Microsoft.AspNetCore.Mvc;
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
        public void Post([FromBody] Episode episode)
        {

        }

        // PUT api/<EpisodesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EpisodesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
