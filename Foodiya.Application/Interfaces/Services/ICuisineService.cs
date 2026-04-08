using Foodiya.Application.DTOs.Cuisine.Request;
using Foodiya.Application.DTOs.Cuisine.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface ICuisineService
{
    Task<CuisineDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<CuisineDetailResponse>> ListAsync(int page, int pageSize, bool? isActive, string? search, CancellationToken ct = default);
    Task<CuisineDetailResponse> CreateAsync(CreateCuisineRequest request, CancellationToken ct = default);
    Task<CuisineDetailResponse> UpdateAsync(int id, UpdateCuisineRequest request, CancellationToken ct = default);
    Task<CuisineDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
