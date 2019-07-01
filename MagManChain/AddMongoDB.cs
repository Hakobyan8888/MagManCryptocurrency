using MagMan;
using MongoDB.Driver;

namespace AddBlock
{
    public class AddMongoDB
    {
        MongoClient dbClient = new MongoClient("mongodb://127.0.0.1:27017");

        public void AddBlock(Block block)
        {
            IMongoDatabase db = dbClient.GetDatabase("Blockchain");
            var collection = db.GetCollection<Block>("ChainOfBlocks");
            var coll = db.ListCollections().ToList();

            collection.InsertOne(block);
            collection.InsertOne(block);
        }
    }
}
