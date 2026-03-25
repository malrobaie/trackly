using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Trackly.Api.DTOs;
using Trackly.Api.Enums;
using Trackly.Api.Interfaces;
using Trackly.Api.Models;

namespace Trackly.Api.Services;

public sealed class UspsTrackingProvider : ITrackingProvider
{
    private static readonly string[] MockStatusSequence =
    [
        "Label Created",
        "Accepted",
        "In Transit",
        "Out For Delivery",
        "Delivered"
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory httpClientFactory;
    private readonly UspsApiOptions options;
    private readonly ILogger<UspsTrackingProvider> logger;
    private readonly SemaphoreSlim tokenLock = new(1, 1);

    private string? accessToken;
    private DateTimeOffset accessTokenExpiresAt = DateTimeOffset.MinValue;

    public UspsTrackingProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<UspsApiOptions> options,
        ILogger<UspsTrackingProvider> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.options = options.Value;
        this.logger = logger;
    }

    public Carrier Carrier => Carrier.Usps;

    public CarrierTrackingSnapshotDto GetSnapshot(string trackingNumber, int stage)
    {
        if (!HasCredentials())
        {
            return GetMockSnapshot(trackingNumber, stage, "USPS API credentials are not configured.");
        }

        try
        {
            return GetSnapshotAsync(trackingNumber).GetAwaiter().GetResult();
        }
        catch (InvalidOperationException)
        {
            if (options.UseMockFallbackWhenUnavailable)
            {
                return GetMockSnapshot(trackingNumber, stage, "USPS API is unavailable, falling back to mock data.");
            }

            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "USPS tracking request failed for tracking number {TrackingNumber}.", trackingNumber);

            if (options.UseMockFallbackWhenUnavailable)
            {
                return GetMockSnapshot(trackingNumber, stage, "Unexpected USPS error, falling back to mock data.");
            }

            throw new InvalidOperationException("USPS tracking is currently unavailable.");
        }
    }

    private async Task<CarrierTrackingSnapshotDto> GetSnapshotAsync(string trackingNumber)
    {
        EnsureCredentials();

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(GetBaseUrl());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync(client));

        using var response = await client.GetAsync($"/tracking/v3/tracking/{trackingNumber}?expand=DETAIL");
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(ExtractErrorMessage(content, response.StatusCode));
        }

        var payload = JsonSerializer.Deserialize<UspsTrackingDetailResponse>(content, JsonOptions)
            ?? throw new InvalidOperationException("USPS tracking returned an empty response.");

        return MapSnapshot(payload, trackingNumber);
    }

    private async Task<string> GetAccessTokenAsync(HttpClient client)
    {
        if (!string.IsNullOrWhiteSpace(accessToken) && accessTokenExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
        {
            return accessToken;
        }

        await tokenLock.WaitAsync();

        try
        {
            if (!string.IsNullOrWhiteSpace(accessToken) && accessTokenExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return accessToken;
            }

            using var tokenResponse = await client.PostAsJsonAsync("/oauth2/v3/token", new
            {
                client_id = options.ClientId,
                client_secret = options.ClientSecret,
                grant_type = "client_credentials"
            });

            var content = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(ExtractErrorMessage(content, tokenResponse.StatusCode));
            }

            var payload = JsonSerializer.Deserialize<UspsTokenResponse>(content, JsonOptions)
                ?? throw new InvalidOperationException("USPS OAuth returned an empty response.");

            accessToken = payload.AccessToken;
            accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn);

            return accessToken;
        }
        finally
        {
            tokenLock.Release();
        }
    }

    private CarrierTrackingSnapshotDto MapSnapshot(UspsTrackingDetailResponse payload, string trackingNumber)
    {
        var events = (payload.TrackingEvents ?? [])
            .Where(trackingEvent => trackingEvent.EventTimestamp.HasValue)
            .OrderByDescending(trackingEvent => trackingEvent.EventTimestamp)
            .Select(trackingEvent => new TrackingEventDto
            {
                Timestamp = trackingEvent.EventTimestamp!.Value,
                Location = BuildLocation(trackingEvent),
                Description = CleanText(trackingEvent.EventType) ?? "Tracking event",
                StatusCode = trackingEvent.EventCode
            })
            .ToList();

        return new CarrierTrackingSnapshotDto
        {
            Carrier = Carrier.Usps,
            TrackingNumber = payload.TrackingNumber ?? trackingNumber,
            Status = BuildStatus(payload),
            EstimatedDelivery = null,
            RetrievedAt = DateTimeOffset.UtcNow,
            Events = events
        };
    }

    private string BuildStatus(UspsTrackingDetailResponse payload)
    {
        var candidate = CleanText(payload.StatusCategory)
            ?? CleanText(payload.Status)
            ?? "USPS status unavailable";

        return candidate switch
        {
            "Accepted" => "Accepted",
            "Delivered" => "Delivered",
            _ => candidate
        };
    }

    private static string BuildLocation(UspsTrackingEvent trackingEvent)
    {
        var parts = new[]
        {
            CleanText(trackingEvent.EventCity),
            CleanText(trackingEvent.EventState),
            CleanText(trackingEvent.EventZip)
        }
        .Where(part => !string.IsNullOrWhiteSpace(part));

        var location = string.Join(", ", parts);
        return string.IsNullOrWhiteSpace(location) ? "USPS Network" : location;
    }

    private static string? CleanText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var withoutTags = Regex.Replace(value, "<.*?>", string.Empty);
        return System.Net.WebUtility.HtmlDecode(withoutTags).Trim();
    }

    private string GetBaseUrl() =>
        options.UseTestEnvironment ? options.TestBaseUrl : options.ProductionBaseUrl;

    private bool HasCredentials() =>
        !string.IsNullOrWhiteSpace(options.ClientId) && !string.IsNullOrWhiteSpace(options.ClientSecret);

    private CarrierTrackingSnapshotDto GetMockSnapshot(string trackingNumber, int stage, string reason)
    {
        logger.LogWarning("USPS live tracking skipped for {TrackingNumber}: {Reason}", trackingNumber, reason);

        var clampedStage = Math.Clamp(stage, 0, MockStatusSequence.Length - 1);
        var now = DateTimeOffset.UtcNow;

        return new CarrierTrackingSnapshotDto
        {
            Carrier = Carrier.Usps,
            TrackingNumber = trackingNumber,
            Status = MockStatusSequence[clampedStage],
            EstimatedDelivery = clampedStage >= MockStatusSequence.Length - 1 ? now : now.AddDays(1),
            RetrievedAt = now,
            Events = new List<TrackingEventDto>
            {
                new()
                {
                    Timestamp = now.AddHours(-36),
                    Location = "Shipping Label Created",
                    Description = "Shipment information received.",
                    StatusCode = "LABEL_CREATED"
                },
                new()
                {
                    Timestamp = now.AddHours(-24),
                    Location = "Origin Facility",
                    Description = "Package accepted by carrier.",
                    StatusCode = "ACCEPTED"
                },
                new()
                {
                    Timestamp = now.AddHours(-12),
                    Location = "USPS Regional Facility",
                    Description = "Package is moving through the carrier network.",
                    StatusCode = "IN_TRANSIT"
                },
                new()
                {
                    Timestamp = now.AddHours(-2),
                    Location = "Destination City",
                    Description = "Package is out for delivery.",
                    StatusCode = "OUT_FOR_DELIVERY"
                },
                new()
                {
                    Timestamp = now.AddMinutes(-20),
                    Location = "Delivery Address",
                    Description = "Package delivered.",
                    StatusCode = "DELIVERED"
                }
            }
            .Take(clampedStage + 1)
            .OrderByDescending(trackingEvent => trackingEvent.Timestamp)
            .ToList()
        };
    }

    private void EnsureCredentials()
    {
        if (HasCredentials())
        {
            return;
        }

        throw new InvalidOperationException(
            "USPS API credentials are missing. Set UspsApi:ClientId and UspsApi:ClientSecret or the UspsApi__ClientId and UspsApi__ClientSecret environment variables.");
    }

    private static string ExtractErrorMessage(string content, System.Net.HttpStatusCode statusCode)
    {
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                var error = JsonSerializer.Deserialize<UspsErrorResponse>(content, JsonOptions);
                var message = error?.ErrorDescription
                    ?? error?.Error
                    ?? error?.Message;

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return $"USPS API error ({(int)statusCode}): {message}";
                }
            }
            catch (JsonException)
            {
            }
        }

        return $"USPS API error ({(int)statusCode}).";
    }

    private sealed class UspsTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int ExpiresIn { get; set; }
    }

    private sealed class UspsTrackingDetailResponse
    {
        [JsonPropertyName("trackingNumber")]
        public string? TrackingNumber { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusCategory")]
        public string? StatusCategory { get; set; }

        [JsonPropertyName("trackingEvents")]
        public List<UspsTrackingEvent>? TrackingEvents { get; set; }
    }

    private sealed class UspsTrackingEvent
    {
        [JsonPropertyName("eventType")]
        public string? EventType { get; set; }

        [JsonPropertyName("eventTimestamp")]
        public DateTimeOffset? EventTimestamp { get; set; }

        [JsonPropertyName("eventCity")]
        public string? EventCity { get; set; }

        [JsonPropertyName("eventState")]
        public string? EventState { get; set; }

        [JsonPropertyName("eventZIP")]
        public string? EventZip { get; set; }

        [JsonPropertyName("eventCode")]
        public string? EventCode { get; set; }
    }

    private sealed class UspsErrorResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
