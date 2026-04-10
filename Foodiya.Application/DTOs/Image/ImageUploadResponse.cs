namespace Foodiya.Application.DTOs.Image;

/// <summary>
/// Returned after a successful image upload.
/// </summary>
public sealed class ImageUploadResponse
{
    public required string Url { get; init; }
}
