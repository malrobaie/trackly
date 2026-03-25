using Trackly.Api.Enums;

namespace Trackly.Api.Services;

public sealed class DhlTrackingProvider : MockTrackingProviderBase
{
    public override Carrier Carrier => Carrier.Dhl;

    protected override string CarrierHub => "DHL Gateway Facility";
}
