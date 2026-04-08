using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeNutrition.Request;
using Foodiya.Application.DTOs.RecipeNutrition.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeNutritionService
{
    Task<RecipeNutritionDetailResponse?> GetByIdAsync(int recipeId, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeNutritionDetailResponse>> ListAsync(int page, int pageSize, int? recipeId, string? search, CancellationToken ct = default);
    Task<RecipeNutritionDetailResponse> CreateAsync(CreateRecipeNutritionRequest request, CancellationToken ct = default);
    Task<RecipeNutritionDetailResponse> UpdateAsync(int recipeId, UpdateRecipeNutritionRequest request, CancellationToken ct = default);
    Task DeleteAsync(int recipeId, CancellationToken ct = default);
}
