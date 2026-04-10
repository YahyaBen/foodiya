namespace Foodiya.Domain.Interfaces.Core;

/// <summary>
/// Port for blob/file storage — the Domain defines WHAT it needs,
/// the Infrastructure decides HOW (Azure, AWS, local disk …).
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a stream and returns the public URL of the stored blob.
    /// </summary>
    /// <param name="folder">Logical folder inside the container (e.g. "cuisines", "recipes").</param>
    /// <param name="stream">File content.</param>
    /// <param name="contentType">MIME type (e.g. "image/webp").</param>
    /// <param name="ct">Cancellation token.</param>
    Task<string> UploadAsync(string folder, Stream stream, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Deletes a blob identified by its public URL. No-op if the blob does not exist.
    /// </summary>
    Task DeleteAsync(string blobUrl, CancellationToken ct = default);
}
