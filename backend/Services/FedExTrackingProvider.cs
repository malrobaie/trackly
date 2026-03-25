using Trackly.Api.Enums;

namespace Trackly.Api.Services;

public sealed class FedExTrackingProvider : MockTrackingProviderBase
{
    public override Carrier Carrier => Carrier.FedEx;

    protected override string CarrierHub => "FedEx Sort Facility";
}
