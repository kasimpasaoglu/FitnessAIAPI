public class LogService : ILogService
{
    private readonly MongoDBClient _mongoDBClient;

    public LogService(MongoDBClient mongoDBClient)
    {
        _mongoDBClient = mongoDBClient;
    }

    public async Task LogSuccess(string action, string collection, object? payload = null)
    {
        var log = new MiniLogModel
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Action = action,
            Payload = payload
        };

        await _mongoDBClient.AddLog(log, collection);
    }

    public async Task LogError(string action, string collection, Exception ex, object? payload = null)
    {
        var log = new MiniLogModel
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Action = action,
            Payload = new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace,
                Input = payload
            }
        };

        await _mongoDBClient.AddLog(log, collection);
    }
}
