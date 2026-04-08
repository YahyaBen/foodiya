using Foodiya.Application.DTOs.PresetAvatarImage.Request;
using Foodiya.Application.DTOs.PresetAvatarImage.Response;
using Foodiya.Application.DTOs.Recipe.Response;

namespace Foodiya.Application.Interfaces.Services;

public interface IPresetAvatarImageService
{
    Task<PresetAvatarImageDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PaginatedResponse<PresetAvatarImageDetailResponse>> ListAsync(int page, int pageSize, bool? isActive, string? search, CancellationToken ct = default);
    Task<PresetAvatarImageDetailResponse> CreateAsync(CreatePresetAvatarImageRequest request, CancellationToken ct = default);
    Task<PresetAvatarImageDetailResponse> UpdateAsync(int id, UpdatePresetAvatarImageRequest request, CancellationToken ct = default);
    Task<PresetAvatarImageDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
