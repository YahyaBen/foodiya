using Foodiya.Application.DTOs.IngredientImage.Request;
using Foodiya.Application.DTOs.IngredientImage.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IIngredientImageService
{
    Task<IngredientImageDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<IngredientImageDetailResponse>> ListAsync(int page, int pageSize, int? ingredientId, bool? isPrimary, string? search, CancellationToken ct = default);
    Task<IngredientImageDetailResponse> CreateAsync(CreateIngredientImageRequest request, CancellationToken ct = default);
    Task<IngredientImageDetailResponse> UpdateAsync(int id, UpdateIngredientImageRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
