using Microsoft.AspNetCore.Mvc;
using Trackly.Api.DTOs;
using Trackly.Api.Interfaces;

namespace Trackly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TrackingController : ControllerBase
{
    private readonly ITrackingService trackingService;

    public TrackingController(ITrackingService trackingService)
    {
        this.trackingService = trackingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TrackedPackageDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public ActionResult<TrackedPackageDetailsDto> Create(CreateTrackingRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TrackingNumber))
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = "Tracking number is required."
            });
        }

        var trackedPackage = trackingService.Create(request);

        return CreatedAtAction(nameof(GetById), new { id = trackedPackage.Id }, trackedPackage);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TrackedPackageSummaryDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<TrackedPackageSummaryDto>> GetAll()
    {
        return Ok(trackingService.GetAll());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TrackedPackageDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public ActionResult<TrackedPackageDetailsDto> GetById(Guid id)
    {
        var trackedPackage = trackingService.GetById(id);

        if (trackedPackage is null)
        {
            return NotFound(new ErrorResponseDto
            {
                Message = $"Tracked package {id} was not found."
            });
        }

        return Ok(trackedPackage);
    }

    [HttpPost("{id:guid}/refresh")]
    [ProducesResponseType(typeof(TrackedPackageDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public ActionResult<TrackedPackageDetailsDto> Refresh(Guid id)
    {
        var trackedPackage = trackingService.Refresh(id);

        if (trackedPackage is null)
        {
            return NotFound(new ErrorResponseDto
            {
                Message = $"Tracked package {id} was not found."
            });
        }

        return Ok(trackedPackage);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!trackingService.Delete(id))
        {
            return NotFound(new ErrorResponseDto
            {
                Message = $"Tracked package {id} was not found."
            });
        }

        return NoContent();
    }
}
