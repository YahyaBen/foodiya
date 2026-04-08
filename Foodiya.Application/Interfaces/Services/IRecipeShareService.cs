using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeShare.Request;
using Foodiya.Application.DTOs.RecipeShare.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeShareService
{
    Task<RecipeShareDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeShareDetailResponse>> ListAsync(int page, int pageSize, int? recipeId, int? sharedByUserId, int? sharedWithUserId, string? channel, string? search, CancellationToken ct = default);
    Task<RecipeShareDetailResponse> CreateAsync(CreateRecipeShareRequest request, CancellationToken ct = default);
    Task<RecipeShareDetailResponse> UpdateAsync(int id, UpdateRecipeShareRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
