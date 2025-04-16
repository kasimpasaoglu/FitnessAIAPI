public interface ILogService
{
    Task LogSuccess(string action, string collection, object? payload = null);
    Task LogError(string action, string collection, Exception exception, object? payload = null);
}