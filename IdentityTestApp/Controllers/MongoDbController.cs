using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace IdentityTestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MongoDbController : ControllerBase
    {
        private IConfiguration _configuration;
        private IMongoClient _client;
        
        public MongoDbController(IConfiguration configuration, IMongoClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        [HttpGet("dbtest")]
        public IActionResult DbTest()
        {
            
            var database = _client.GetDatabase(_configuration.GetConnectionString("DatabaseName"));
            var collection = database.GetCollection<BsonDocument>(_configuration.GetConnectionString("CollectionName"));
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "10006546");
            var data = collection.Find(filter).ToList();
            
            return Ok(data);

            /*var connectionString = _configuration.GetConnectionString("MongoDb");
            if (connectionString == null)
            {
                Console.WriteLine(
                    "You must set your 'MONGODB_URI' environmental variable. See\n\t https://www.mongodb.com/docs/drivers/go/current/usage-examples/#environment-variable");
                Environment.Exit(0);
            }

            var client = new MongoClient(connectionString);
            var collection = client.GetDatabase("sample_mflix").GetCollection<BsonDocument>("movies");
            var filter = Builders<BsonDocument>.Filter.Eq("title", "Back to the Future");
            var document = collection.Find(filter).First();
            Console.WriteLine(document);
            return Ok(document);*/
        }

    }

}
