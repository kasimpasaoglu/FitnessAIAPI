using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDBClient
{
    private readonly IMongoDatabase _database;
    public MongoDBClient(string connectionString)
    {
        var mongoClient = new MongoClient(connectionString);
        _database = mongoClient.GetDatabase("FitnessAI");
    }
    public async Task AddLog<T>(T logEntry, string collectionName) where T : ILogEntry
    {
        var fullCollectionName = $"{collectionName}_{DateTime.UtcNow:yyyy_MM_dd}";
        var collection = _database.GetCollection<T>(fullCollectionName);
        await collection.InsertOneAsync(logEntry);
    }
}