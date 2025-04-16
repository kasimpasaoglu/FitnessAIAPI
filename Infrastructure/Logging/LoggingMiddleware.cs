using System.Diagnostics;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MongoDBClient _mongoDBClient;

    public LoggingMiddleware(RequestDelegate next, MongoDBClient mongoDBClient)
    {
        _next = next;
        _mongoDBClient = mongoDBClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        string? exceptionMessage = null;
        string? stackTrace = null;

        try
        {
            await _next(context); // controller'a geç
        }
        catch (Exception ex)
        {
            // Exception yakala ama tekrar fırlat (böylece hata client'a gider)
            exceptionMessage = ex.Message;
            stackTrace = ex.StackTrace;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            var routeValues = context.Request.RouteValues.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.ToString() ?? string.Empty
            );

            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            var ip = !string.IsNullOrEmpty(forwardedFor)
                ? forwardedFor.Split(',')[0].Trim()
                : context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var log = new LogModel
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Method = context.Request.Method,
                Path = context.Request.Path,
                QueryString = context.Request.QueryString.Value,
                RouteValues = routeValues,
                IP = ip,
                ResponseStatusCode = context.Response.StatusCode,
                ProcessDuration = stopwatch.ElapsedMilliseconds,
                Action = exceptionMessage != null ? "Error" : "Request",
                Payload = exceptionMessage != null ? new
                {
                    Error = exceptionMessage,
                    StackTrace = stackTrace
                } : null
            };

            await _mongoDBClient.AddLog(log, "HttpLogs");
        }
    }
}