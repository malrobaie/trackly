using System.Collections.Concurrent;
using Trackly.Api.DTOs;
using Trackly.Api.Enums;
using Trackly.Api.Interfaces;
using Trackly.Api.Models;

namespace Trackly.Api.Services;

public sealed class InMemoryTrackingService : ITrackingService
{
    private static readonly string[] StatusSequence =
    [
        "Label Created",
        "Accepted",
        "In Transit",
        "Out For Delivery",
        "Delivered"
    ];

    private readonly ConcurrentDictionary<Guid, TrackedPackage> packages = new();

    public TrackedPackageDetailsDto Create(CreateTrackingRequestDto request)
    {
        var normalizedTrackingNumber = NormalizeTrackingNumber(request.TrackingNumber);
        var package = BuildMockPackage(request.Carrier, normalizedTrackingNumber, 2);

        packages[package.Id] = package;

        return MapDetails(package);
    }

    public IReadOnlyList<TrackedPackageSummaryDto> GetAll() =>
        packages.Values
            .OrderByDescending(packageItem => packageItem.LastUpdated)
            .Select(MapSummary)
            .ToList();

    public TrackedPackageDetailsDto? GetById(Guid id) =>
        packages.TryGetValue(id, out var package)
            ? MapDetails(package)
            : null;

    public TrackedPackageDetailsDto? Refresh(Guid id)
    {
        if (!packages.TryGetValue(id, out var existing))
        {
            return null;
        }

        var currentStage = GetStageIndex(existing.Status);
        var nextStage = Math.Min(currentStage + 1, StatusSequence.Length - 1);
        var refreshed = BuildMockPackage(existing.Carrier, existing.TrackingNumber, nextStage, existing.Id);

        packages[id] = refreshed;

        return MapDetails(refreshed);
    }

    public bool Delete(Guid id) => packages.TryRemove(id, out _);

    private static TrackedPackage BuildMockPackage(Carrier carrier, string trackingNumber, int stage, Guid? id = null)
    {
        var clampedStage = Math.Clamp(stage, 0, StatusSequence.Length - 1);
        var now = DateTimeOffset.UtcNow;
        var events = BuildEvents(carrier, clampedStage, now);

        return new TrackedPackage
        {
            Id = id ?? Guid.NewGuid(),
            Carrier = carrier,
            TrackingNumber = trackingNumber,
            Status = StatusSequence[clampedStage],
            EstimatedDelivery = clampedStage >= StatusSequence.Length - 1 ? now : now.AddDays(1),
            LastUpdated = now,
            Events = events
        };
    }

    private static List<TrackingEvent> BuildEvents(Carrier carrier, int stage, DateTimeOffset now)
    {
        var carrierHub = carrier switch
        {
            Carrier.Usps => "USPS Regional Facility",
            Carrier.Ups => "UPS Distribution Center",
            _ => "Carrier Facility"
        };

        var allEvents = new List<TrackingEvent>
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
                Location = carrierHub,
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

    private static string NormalizeTrackingNumber(string value) =>
        value.Trim().ToUpperInvariant();

    private static int GetStageIndex(string status)
    {
        var index = Array.IndexOf(StatusSequence, status);
        return index < 0 ? 0 : index;
    }

    private static TrackedPackageSummaryDto MapSummary(TrackedPackage package) =>
        new()
        {
            Id = package.Id,
            Carrier = package.Carrier,
            TrackingNumber = package.TrackingNumber,
            Status = package.Status,
            EstimatedDelivery = package.EstimatedDelivery,
            LastUpdated = package.LastUpdated
        };

    private static TrackedPackageDetailsDto MapDetails(TrackedPackage package) =>
        new()
        {
            Id = package.Id,
            Carrier = package.Carrier,
            TrackingNumber = package.TrackingNumber,
            Status = package.Status,
            EstimatedDelivery = package.EstimatedDelivery,
            LastUpdated = package.LastUpdated,
            Events = package.Events
                .OrderByDescending(trackingEvent => trackingEvent.Timestamp)
                .Select(trackingEvent => new TrackingEventDto
                {
                    Timestamp = trackingEvent.Timestamp,
                    Location = trackingEvent.Location,
                    Description = trackingEvent.Description,
                    StatusCode = trackingEvent.StatusCode
                })
                .ToList()
        };
}
