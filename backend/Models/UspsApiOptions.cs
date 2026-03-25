namespace Trackly.Api.Models;

public sealed class UspsApiOptions
{
    public const string SectionName = "UspsApi";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public bool UseTestEnvironment { get; set; }

    public bool UseMockFallbackWhenUnavailable { get; set; } = true;

    public string ProductionBaseUrl { get; set; } = "https://apis.usps.com";

    public string TestBaseUrl { get; set; } = "https://apis-tem.usps.com";
}
