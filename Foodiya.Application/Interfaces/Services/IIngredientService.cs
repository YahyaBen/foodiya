using Foodiya.Application.DTOs.Ingredient.Request;
using Foodiya.Application.DTOs.Ingredient.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IIngredientService
{
    Task<IngredientDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<IngredientDetailResponse>> ListAsync(int page, int pageSize, int? ingredientTypeId, bool? isActive, string? search, CancellationToken ct = default);
    Task<IngredientDetailResponse> CreateAsync(CreateIngredientRequest request, CancellationToken ct = default);
    Task<IngredientDetailResponse> UpdateAsync(int id, UpdateIngredientRequest request, CancellationToken ct = default);
    Task<IngredientDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
