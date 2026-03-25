using Trackly.Api.DTOs;
using Trackly.Api.Enums;
using Trackly.Api.Interfaces;

namespace Trackly.Api.Services;

public abstract class MockTrackingProviderBase : ITrackingProvider
{
    protected static readonly string[] StatusSequence =
    [
        "Label Created",
        "Accepted",
        "In Transit",
        "Out For Delivery",
        "Delivered"
    ];

    public abstract Carrier Carrier { get; }

    protected abstract string CarrierHub { get; }

    public CarrierTrackingSnapshotDto GetSnapshot(string trackingNumber, int stage)
    {
        var clampedStage = Math.Clamp(stage, 0, StatusSequence.Length - 1);
        var now = DateTimeOffset.UtcNow;

        return new CarrierTrackingSnapshotDto
        {
            Carrier = Carrier,
            TrackingNumber = trackingNumber,
            Status = StatusSequence[clampedStage],
            EstimatedDelivery = clampedStage >= StatusSequence.Length - 1 ? now : now.AddDays(1),
            RetrievedAt = now,
            Events = BuildEvents(clampedStage, now)
        };
    }

    private List<TrackingEventDto> BuildEvents(int stage, DateTimeOffset now)
    {
        var allEvents = new List<TrackingEventDto>
        {
            new()
            {
                Timestamp = now.AddHours(-36),
                Location = "Shipping Label Created",
                Description = "Shipment information received.",
                StatusCode = "LABEL_CREATED"
            },
            new()
            {
                Timestamp = now.AddHours(-24),
                Location = "Origin Facility",
                Description = "Package accepted by carrier.",
                StatusCode = "ACCEPTED"
            },
            new()
            {
                Timestamp = now.AddHours(-12),
                Location = CarrierHub,
                Description = "Package is moving through the carrier network.",
                StatusCode = "IN_TRANSIT"
            },
            new()
            {
                Timestamp = now.AddHours(-2),
                Location = "Destination City",
                Description = "Package is out for delivery.",
                StatusCode = "OUT_FOR_DELIVERY"
            },
            new()
            {
                Timestamp = now.AddMinutes(-20),
                Location = "Delivery Address",
                Description = "Package delivered.",
                StatusCode = "DELIVERED"
            }
        };

        return allEvents
            .Take(stage + 1)
            .OrderByDescending(trackingEvent => trackingEvent.Timestamp)
            .ToList();
    }
}
