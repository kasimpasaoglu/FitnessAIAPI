
using System.Diagnostics;
using System.Text.Json;

public class LogService : ILogService
{

    private readonly MongoDBClient _mongoDBClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogService(MongoDBClient mongoDBClient, IHttpContextAccessor httpContextAccessor)
    {
        _mongoDBClient = mongoDBClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogRequest(string action, string collection, long duration)
    {
        var ctx = _httpContextAccessor.HttpContext;
        var log = new LogModel
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Method = ctx?.Request?.Method ?? "Unknown",
            Path = ctx?.Request?.Path.Value ?? "Unknown",
            QueryString = ctx?.Request?.QueryString.Value ?? "Unknown",
            RouteValues = ctx?.Request?.RouteValues.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.ToString() ?? string.Empty
                ),
            IP = ctx?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            ResponseStatusCode = ctx?.Response?.StatusCode ?? 00000,
            ProcessDuration = duration,
            Action = action,
        };
        await _mongoDBClient.AddLog(log, collection);
    }

    public async Task LogSuccess(string action, string collection, object? payload = null)
    {
        var ctx = _httpContextAccessor.HttpContext;
        var log = new LogModel
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Method = ctx?.Request?.Method ?? "Unknown",
            Path = ctx?.Request?.Path.Value ?? "Unknown",
            IP = ctx?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            ResponseStatusCode = ctx?.Response?.StatusCode ?? 00000,
            Action = action,
            Payload = payload != null ? JsonSerializer.Serialize(payload) : null
        };

        await _mongoDBClient.AddLog(log, collection);
    }

    public async Task LogError(string action, string collection, Exception ex, object? payload = null)
    {
        var ctx = _httpContextAccessor.HttpContext;
        var log = new LogModel
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Method = ctx?.Request?.Method ?? "Unknown",
            Path = ctx?.Request?.Path.Value ?? "Unknown",
            IP = ctx?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            ResponseStatusCode = ctx?.Response?.StatusCode ?? 00000,
            Action = action,
            Payload = JsonSerializer.Serialize(new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace,
                Input = payload
            })
        };

        await _mongoDBClient.AddLog(log, collection);
    }



}
