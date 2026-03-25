using Trackly.Api.DTOs;
using Trackly.Api.Enums;

namespace Trackly.Api.Interfaces;

public interface ITrackingOrchestratorService
{
    CarrierTrackingSnapshotDto GetSnapshot(Carrier carrier, string trackingNumber, int stage);
}
