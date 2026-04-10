using Foodiya.Application.DTOs.Image;
using Foodiya.Application.Interfaces.Services;
using Foodiya.Domain.Exceptions;
using Foodiya.Domain.Interfaces.Core;

namespace Foodiya.Application.Services;

public sealed class ImageUploadService : IImageUploadService
{
    private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2 MB

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
        "image/svg+xml"
    };

    private static readonly HashSet<string> AllowedFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        "users",
        "chefs",
        "recipes",
        "ingredients",
        "cuisines",
        "difficulties",
        "cities",
        "regions",
        "avatars"
    };

    private readonly IBlobStorageService _blobStorage;

    public ImageUploadService(IBlobStorageService blobStorage)
    {
        _blobStorage = blobStorage;
    }

    public async Task<ImageUploadResponse> UploadAsync(
        string folder,
        Stream stream,
        string contentType,
        long fileSize,
        CancellationToken ct = default)
    {
        if (!AllowedFolders.Contains(folder))
            throw new FoodiyaBadRequestException($"Invalid folder '{folder}'. Allowed: {string.Join(", ", AllowedFolders)}.");

        if (!AllowedContentTypes.Contains(contentType))
            throw new FoodiyaBadRequestException($"File type '{contentType}' is not allowed. Allowed: {string.Join(", ", AllowedContentTypes)}.");

        if (fileSize > MaxFileSizeBytes)
            throw new FoodiyaBadRequestException($"File size exceeds the {MaxFileSizeBytes / (1024 * 1024)} MB limit.");

        var url = await _blobStorage.UploadAsync(folder, stream, contentType, ct);

        return new ImageUploadResponse { Url = url };
    }

    public Task DeleteAsync(string imageUrl, CancellationToken ct = default)
    {
        return _blobStorage.DeleteAsync(imageUrl, ct);
    }
}
