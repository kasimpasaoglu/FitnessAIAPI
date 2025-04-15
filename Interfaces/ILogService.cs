public interface ILogService
{
    Task LogRequest(string action, string collection, long duration);
    Task LogSuccess(string action, string collection, object? payload = null);
    Task LogError(string action, string collection, Exception exception, object? payload = null);
}