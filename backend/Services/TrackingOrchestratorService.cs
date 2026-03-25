using Trackly.Api.DTOs;
using Trackly.Api.Enums;
using Trackly.Api.Interfaces;

namespace Trackly.Api.Services;

public sealed class TrackingOrchestratorService : ITrackingOrchestratorService
{
    private readonly IReadOnlyDictionary<Carrier, ITrackingProvider> providers;

    public TrackingOrchestratorService(IEnumerable<ITrackingProvider> providers)
    {
        this.providers = providers.ToDictionary(provider => provider.Carrier);
    }

    public CarrierTrackingSnapshotDto GetSnapshot(Carrier carrier, string trackingNumber, int stage)
    {
        if (!providers.TryGetValue(carrier, out var provider))
        {
            throw new InvalidOperationException($"No tracking provider is registered for carrier {carrier}.");
        }

        return provider.GetSnapshot(trackingNumber, stage);
    }
}
