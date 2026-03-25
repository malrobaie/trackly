namespace Trackly.Api.Models;

public sealed class TrackingEvent
{
    public DateTimeOffset Timestamp { get; set; }

    public string Location { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? StatusCode { get; set; }
}
