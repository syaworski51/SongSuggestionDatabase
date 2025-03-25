using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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



        public async Task InsertDocument(IMongoCollection<object> collection, object document)
        {
            await collection.InsertOneAsync(document);
        }

        public async Task InsertDocument(IMongoCollection<object> collection, IEnumerable<object> documents)
        {
            await collection.InsertManyAsync(documents);
        }

        public async Task<object> GetDocument(IMongoCollection<object> collection, ObjectId id)
        {
            var filter = Builders<object>.Filter.Eq("Id", id);
            var document = await collection.Find(filter).FirstOrDefaultAsync();

            if (document == null)
                throw new Exception($"A document with the ID {id} could not be found in the specified collection.");
            
            return document;
        }

        public async Task<object> GetDocuments(IMongoCollection<object> collection, FilterDefinition<object> filter)
        {
            return await collection.FindAsync(filter);
        }

        public async Task<bool> ReplaceDocument(IMongoCollection<object> collection, ObjectId id, object replacement)
        {
            var filter = Builders<object>.Filter.Eq("Id", id);
            var updateResult = await collection.ReplaceOneAsync(filter, replacement);

            return updateResult.ModifiedCount > 0; 
        }

        public async Task<bool> DeleteDocument(IMongoCollection<object> collection, ObjectId id)
        {
            var filter = Builders<object>.Filter.Eq("Id", id);
            var result = await collection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteDocuments(IMongoCollection<object> collection, FilterDefinition<object> filter)
        {
            var result = await collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}
