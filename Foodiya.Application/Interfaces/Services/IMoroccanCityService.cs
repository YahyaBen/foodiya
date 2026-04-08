using Foodiya.Application.DTOs.MoroccanCity.Request;
using Foodiya.Application.DTOs.MoroccanCity.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IMoroccanCityService
{
    Task<MoroccanCityDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<MoroccanCityDetailResponse>> ListAsync(int page, int pageSize, int? regionId, bool? isActive, string? search, CancellationToken ct = default);
    Task<MoroccanCityDetailResponse> CreateAsync(CreateMoroccanCityRequest request, CancellationToken ct = default);
    Task<MoroccanCityDetailResponse> UpdateAsync(int id, UpdateMoroccanCityRequest request, CancellationToken ct = default);
    Task<MoroccanCityDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
