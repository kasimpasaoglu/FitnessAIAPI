public interface ILogEntry
{
    public string Date { get; set; }
    public string? Action { get; set; }
    public object? Payload { get; set; }
}