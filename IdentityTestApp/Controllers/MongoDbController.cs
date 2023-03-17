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
        private IMongoDatabase _database;
        
        public MongoDbController(IConfiguration configuration, IMongoDatabase database)
        {
            _configuration = configuration;
            _database = database;
        }

        [HttpGet("dbtest")]
        public IActionResult DbTest()
        {
            
            var data = _database.GetCollection<BsonDocument>("movies");

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
