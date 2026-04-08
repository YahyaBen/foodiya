using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeLike.Request;
using Foodiya.Application.DTOs.RecipeLike.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IRecipeLikeService
{
    Task<RecipeLikeDetailResponse?> GetByIdAsync(int recipeId, int userId, CancellationToken ct = default);
    Task<PaginatedResponse<RecipeLikeDetailResponse>> ListAsync(int page, int pageSize, int? recipeId, int? userId, string? search, CancellationToken ct = default);
    Task<RecipeLikeDetailResponse> CreateAsync(CreateRecipeLikeRequest request, CancellationToken ct = default);
    Task DeleteAsync(int recipeId, int userId, CancellationToken ct = default);
}
