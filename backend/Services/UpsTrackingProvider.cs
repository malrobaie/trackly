using Trackly.Api.Enums;

namespace Trackly.Api.Services;

public sealed class UpsTrackingProvider : MockTrackingProviderBase
{
    public override Carrier Carrier => Carrier.Ups;

    protected override string CarrierHub => "UPS Distribution Center";
}
