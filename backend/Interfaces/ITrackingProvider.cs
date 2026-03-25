using Trackly.Api.DTOs;
using Trackly.Api.Enums;

namespace Trackly.Api.Interfaces;

public interface ITrackingProvider
{
    Carrier Carrier { get; }

    CarrierTrackingSnapshotDto GetSnapshot(string trackingNumber, int stage);
}
