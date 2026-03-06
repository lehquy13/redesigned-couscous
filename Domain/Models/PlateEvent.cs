namespace EventFlowInspector.Domain.Models;

public sealed class PlateEvent
{
    public string EventId { get; init; } = string.Empty;
    public string Plate { get; init; } = string.Empty;
    public string CameraId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string VehicleType { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}
