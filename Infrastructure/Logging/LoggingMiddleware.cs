using System.Diagnostics;
using System.Text.Json;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MongoDBClient _mongoDBClient;

    public LoggingMiddleware(RequestDelegate next, MongoDBClient mongoDBClient)
    {
        _next = next;
        _mongoDBClient = mongoDBClient;
    }

    private object? TryDeserializeAndSanitize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var obj = JsonSerializer.Deserialize<JsonElement>(json, options);
            return SanitizeGuids(obj);
        }
        catch
        {
            return json;
        }
    }

    private object SanitizeGuids(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    dict[prop.Name] = SanitizeGuids(prop.Value);
                }
                return dict;

            case JsonValueKind.Array:
                var list = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(SanitizeGuids(item));
                }
                return list;

            case JsonValueKind.String:
                if (Guid.TryParse(element.GetString(), out var guid))
                    return guid.ToString(); // force string

                return element.GetString();

            case JsonValueKind.Number:
                return element.GetDouble();

            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();

            case JsonValueKind.Null:
            default:
                return null;
        }
    }



    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        string? exceptionMessage = null;
        string? stackTrace = null;
        string? requestBody = null;
        string? responseBody = null;

        // Response'u yakalayabilmek için stream'i değiştiriyoruz
        var originalResponseBodyStream = context.Response.Body;
        await using var newResponseBody = new MemoryStream();
        context.Response.Body = newResponseBody;

        try
        {
            // Request body'sini oku (stream başa sarılmalı)
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            await _next(context); // pipeline'a devam et
        }
        catch (Exception ex)
        {
            exceptionMessage = ex.Message;
            stackTrace = ex.StackTrace;
            throw; // tekrar fırlat ki client 500 alabilsin
        }
        finally
        {
            stopwatch.Stop();

            // Response body'yi oku
            newResponseBody.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(newResponseBody).ReadToEndAsync();
            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBodyStream); // orijinal stream'e geri yaz

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
                Payload = new
                {
                    Error = exceptionMessage,
                    StackTrace = stackTrace,
                    RequestBody = TryDeserializeAndSanitize(requestBody),
                    ResponseBody = TryDeserializeAndSanitize(responseBody)
                }
            };

            await _mongoDBClient.AddLog(log, "HttpLogs");
        }
    }
}