using System.Collections.Concurrent;
using Trackly.Api.DTOs;
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
    private readonly ITrackingOrchestratorService trackingOrchestrator;

    public InMemoryTrackingService(ITrackingOrchestratorService trackingOrchestrator)
    {
        this.trackingOrchestrator = trackingOrchestrator;
    }

    public TrackedPackageDetailsDto Create(CreateTrackingRequestDto request)
    {
        var normalizedTrackingNumber = NormalizeTrackingNumber(request.TrackingNumber);
        var snapshot = trackingOrchestrator.GetSnapshot(request.Carrier, normalizedTrackingNumber, 2);
        var package = BuildPackage(snapshot);

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
        var snapshot = trackingOrchestrator.GetSnapshot(existing.Carrier, existing.TrackingNumber, nextStage);
        var refreshed = BuildPackage(snapshot, existing.Id);

        packages[id] = refreshed;

        return MapDetails(refreshed);
    }

    public bool Delete(Guid id) => packages.TryRemove(id, out _);

    private static TrackedPackage BuildPackage(CarrierTrackingSnapshotDto snapshot, Guid? id = null)
    {
        return new TrackedPackage
        {
            Id = id ?? Guid.NewGuid(),
            Carrier = snapshot.Carrier,
            TrackingNumber = snapshot.TrackingNumber,
            Status = snapshot.Status,
            EstimatedDelivery = snapshot.EstimatedDelivery,
            LastUpdated = snapshot.RetrievedAt,
            Events = snapshot.Events
                .Select(trackingEvent => new TrackingEvent
                {
                    Timestamp = trackingEvent.Timestamp,
                    Location = trackingEvent.Location,
                    Description = trackingEvent.Description,
                    StatusCode = trackingEvent.StatusCode
                })
                .ToList()
        };
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
