using MongoDB.Driver;

namespace SSDBAPI.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var secrets = configuration.GetSection("Secrets:MongoDB");
            var connectionString = secrets.GetValue<string>("ConnectionString");
            var name = secrets.GetValue<string>("DBName");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(name);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
