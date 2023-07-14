using MongoDB.Driver;

namespace SlotMachine.Infrastructure
{
    public static class DefaultMongoClient
    {
        public static MongoClient GetMongoClient(IMongoDbSettings settings)
        {
            var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            var database = new MongoClient(mongoSettings);
                //.GetDatabase(settings.DatabaseName);

            return database;
        }
    }
}
