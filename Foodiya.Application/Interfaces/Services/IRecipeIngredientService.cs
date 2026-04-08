using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeIngredient.Request;
using Foodiya.Application.DTOs.RecipeIngredient.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeIngredientService
{
    Task<RecipeIngredientDetailResponse?> GetByIdAsync(int recipeId, int ingredientId, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeIngredientDetailResponse>> ListAsync(int page, int pageSize, int? recipeId, int? ingredientId, int? unitId, string? search, CancellationToken ct = default);
    Task<RecipeIngredientDetailResponse> CreateAsync(CreateRecipeIngredientRequest request, CancellationToken ct = default);
    Task<RecipeIngredientDetailResponse> UpdateAsync(int recipeId, int ingredientId, UpdateRecipeIngredientRequest request, CancellationToken ct = default);
    Task DeleteAsync(int recipeId, int ingredientId, CancellationToken ct = default);
}
