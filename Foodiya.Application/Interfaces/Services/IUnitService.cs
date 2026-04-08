using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.Unit.Request;
using Foodiya.Application.DTOs.Unit.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IUnitService
{
    Task<UnitDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<UnitDetailResponse>> ListAsync(int page, int pageSize, bool? isActive, string? search, CancellationToken ct = default);
    Task<UnitDetailResponse> CreateAsync(CreateUnitRequest request, CancellationToken ct = default);
    Task<UnitDetailResponse> UpdateAsync(int id, UpdateUnitRequest request, CancellationToken ct = default);
    Task<UnitDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
