using Foodiya.Application.DTOs.Image;

namespace Foodiya.Application.Interfaces.Services;

/// <summary>
/// Application-level service that validates, delegates to blob storage, and returns the URL.
/// Keeps controllers thin and business rules in the Application layer.
/// </summary>
public interface IImageUploadService
{
    /// <summary>
    /// Validates and uploads an image file.
    /// </summary>
    /// <param name="folder">Target folder (e.g. "cuisines", "recipes").</param>
    /// <param name="stream">File stream.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="fileSize">File size in bytes.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<ImageUploadResponse> UploadAsync(string folder, Stream stream, string contentType, long fileSize, CancellationToken ct = default);

    /// <summary>
    /// Deletes a previously-uploaded image by its URL.
    /// </summary>
    Task DeleteAsync(string imageUrl, CancellationToken ct = default);
}
