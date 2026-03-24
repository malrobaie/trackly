namespace Trackly.Api.DTOs;

public sealed class TrackingEventDto
{
    public DateTimeOffset Timestamp { get; set; }

    public string Location { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? StatusCode { get; set; }
}
