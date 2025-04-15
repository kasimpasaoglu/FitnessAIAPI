using MongoDB.Bson;

public interface ILogEntry
{
    BsonDocument ToBsonDocument();
}