using Trackly.Api.DTOs;

namespace Trackly.Api.Interfaces;

public interface ITrackingService
{
    TrackedPackageDetailsDto Create(CreateTrackingRequestDto request);

    IReadOnlyList<TrackedPackageSummaryDto> GetAll();

    TrackedPackageDetailsDto? GetById(Guid id);

    TrackedPackageDetailsDto? Refresh(Guid id);

    bool Delete(Guid id);
}
