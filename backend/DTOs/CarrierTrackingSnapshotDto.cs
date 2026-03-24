using Trackly.Api.Enums;

namespace Trackly.Api.DTOs;

public sealed class CarrierTrackingSnapshotDto
{
    public Carrier Carrier { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTimeOffset? EstimatedDelivery { get; set; }

    public DateTimeOffset RetrievedAt { get; set; }

    public List<TrackingEventDto> Events { get; set; } = [];
}
