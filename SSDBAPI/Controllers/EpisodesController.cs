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

        public EpisodesController(MongoDbContext context)
        {
            _context = context;
        }
        
        
        // GET: api/<EpisodesController>
        [HttpGet]
        public List<Episode> Get(string order = "desc")
        {
            var episodesCollection = _context.GetCollection<Episode>("episodes");
            
            var episodes = episodesCollection.Find(_ => true)
                .SortByDescending(e => e.Date)
                .ToList();

            return episodesCollection;
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
