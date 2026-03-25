using System.ComponentModel.DataAnnotations;
using Trackly.Api.Enums;

namespace Trackly.Api.DTOs;

public sealed class CreateTrackingRequestDto
{
    [Required]
    public Carrier Carrier { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(64)]
    public string TrackingNumber { get; set; } = string.Empty;
}
