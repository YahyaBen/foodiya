using Foodiya.Application.DTOs.ChefProfile.Request;
using Foodiya.Application.DTOs.ChefProfile.Response;
using Foodiya.Application.DTOs.Common;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IChefProfileService
{
    Task<ChefProfileDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<ChefProfileDetailResponse>> ListAsync(int page, int pageSize, bool? isVerified, string? search, CancellationToken ct = default);
    Task<ChefProfileDetailResponse> CreateAsync(int userId, CreateChefProfileRequest request, CancellationToken ct = default);
    Task<ChefProfileDetailResponse> UpdateAsync(int id, UpdateChefProfileRequest request, CancellationToken ct = default);
    Task<ChefProfileDetailResponse> ToggleVerifiedAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, SoftDeleteRequest request, CancellationToken ct = default);
}
