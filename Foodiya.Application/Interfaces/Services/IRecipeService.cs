using Foodiya.Application.DTOs.Common;
using Foodiya.Application.DTOs.Recipe.Request;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeService
{
    Task<RecipeDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeDetailResponse>> ListAsync(int page, int pageSize, int? cuisineId, int? difficultyId, int? categoryId, string? search, CancellationToken ct = default);
    Task<RecipeDetailResponse> CreateAsync(int chefId, CreateRecipeRequest request, CancellationToken ct = default);
    Task<RecipeDetailResponse> UpdateAsync(int id, UpdateRecipeRequest request, CancellationToken ct = default);
    Task<RecipeDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, SoftDeleteRequest request, CancellationToken ct = default);
}
