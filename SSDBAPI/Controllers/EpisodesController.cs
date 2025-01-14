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
        public async Task<ActionResult<List<Episode>>> Get(string order = "desc")
        {
            if (_collection == null)
                return NotFound();

            var episodes = _collection.Find(_ => true)
                .SortByDescending(e => e.Date);

            return await episodes.ToListAsync();
        }

        // GET api/<EpisodesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EpisodesController>
        [HttpPost]
        public void Post([FromBody] string value)
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
