using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeStep.Request;
using Foodiya.Application.DTOs.RecipeStep.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeStepService
{
    Task<RecipeStepDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeStepDetailResponse>> ListAsync(int page, int pageSize, int? recipeId, string? search, CancellationToken ct = default);
    Task<RecipeStepDetailResponse> CreateAsync(CreateRecipeStepItemRequest request, CancellationToken ct = default);
    Task<RecipeStepDetailResponse> UpdateAsync(int id, UpdateRecipeStepItemRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
