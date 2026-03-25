using Trackly.Api.Enums;

namespace Trackly.Api.Services;

public sealed class UspsTrackingProvider : MockTrackingProviderBase
{
    public override Carrier Carrier => Carrier.Usps;

    protected override string CarrierHub => "USPS Regional Facility";
}
