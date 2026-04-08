using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeImage.Request;
using Foodiya.Application.DTOs.RecipeImage.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeImageService
{
    Task<RecipeImageDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeImageDetailResponse>> ListAsync(int page, int pageSize, int? recipeId, bool? isPrimary, string? search, CancellationToken ct = default);
    Task<RecipeImageDetailResponse> CreateAsync(CreateRecipeImageRequest request, CancellationToken ct = default);
    Task<RecipeImageDetailResponse> UpdateAsync(int id, UpdateRecipeImageRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
