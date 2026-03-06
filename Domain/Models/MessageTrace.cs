namespace EventFlowInspector.Domain.Models;

public sealed class MessageTrace
{
    public string Plate { get; init; } = string.Empty;
    public string Queue { get; init; } = string.Empty;
    public int Partition { get; init; }
    public long Offset { get; init; }
    public DateTime Timestamp { get; init; }
    public string? EventId { get; init; }
    public string RawMessage { get; init; } = string.Empty;
}
