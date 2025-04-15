using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public LoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew(); // sayaci baslat

        await _next(context); // istegi isle

        stopwatch.Stop(); // sayaci durdur
        using (var scope = _scopeFactory.CreateScope())
        {
            var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
            await logService.LogRequest("Request Tracked", "GenericLog", stopwatch.ElapsedMilliseconds);
        }
    }
}
