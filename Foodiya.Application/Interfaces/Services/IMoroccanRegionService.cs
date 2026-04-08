using Foodiya.Application.DTOs.MoroccanRegion.Request;
using Foodiya.Application.DTOs.MoroccanRegion.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IMoroccanRegionService
{
    Task<MoroccanRegionDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<MoroccanRegionDetailResponse>> ListAsync(int page, int pageSize, bool? isActive, string? search, CancellationToken ct = default);
    Task<MoroccanRegionDetailResponse> CreateAsync(CreateMoroccanRegionRequest request, CancellationToken ct = default);
    Task<MoroccanRegionDetailResponse> UpdateAsync(int id, UpdateMoroccanRegionRequest request, CancellationToken ct = default);
    Task<MoroccanRegionDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
