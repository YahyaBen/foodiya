using Foodiya.Application.DTOs.IngredientType.Request;
using Foodiya.Application.DTOs.IngredientType.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IIngredientTypeService
{
    Task<IngredientTypeDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<IngredientTypeDetailResponse>> ListAsync(int page, int pageSize, bool? isActive, string? search, CancellationToken ct = default);
    Task<IngredientTypeDetailResponse> CreateAsync(CreateIngredientTypeRequest request, CancellationToken ct = default);
    Task<IngredientTypeDetailResponse> UpdateAsync(int id, UpdateIngredientTypeRequest request, CancellationToken ct = default);
    Task<IngredientTypeDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
