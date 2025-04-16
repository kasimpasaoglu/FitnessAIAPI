public class MiniLogModel : ILogEntry
{
    public string Date { get; set; } = null!;
    public string? Action { get; set; }
    public object? Payload { get; set; }
}