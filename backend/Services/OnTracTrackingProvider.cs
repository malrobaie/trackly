using Trackly.Api.Enums;

namespace Trackly.Api.Services;

public sealed class OnTracTrackingProvider : MockTrackingProviderBase
{
    public override Carrier Carrier => Carrier.OnTrac;

    protected override string CarrierHub => "OnTrac Regional Hub";
}
