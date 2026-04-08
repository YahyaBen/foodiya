using Foodiya.Application.DTOs.Difficulty.Request;
using Foodiya.Application.DTOs.Difficulty.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IDifficultyService
{
    Task<DifficultyDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<DifficultyDetailResponse>> ListAsync(int page, int pageSize, bool? isActive, string? search, CancellationToken ct = default);
    Task<DifficultyDetailResponse> CreateAsync(CreateDifficultyRequest request, CancellationToken ct = default);
    Task<DifficultyDetailResponse> UpdateAsync(int id, UpdateDifficultyRequest request, CancellationToken ct = default);
    Task<DifficultyDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
