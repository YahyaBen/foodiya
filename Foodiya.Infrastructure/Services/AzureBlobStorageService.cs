using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Foodiya.Domain.Configuration;
using Foodiya.Domain.Interfaces.Core;
using Microsoft.Extensions.Options;

namespace Foodiya.Infrastructure.Services;

/// <summary>
/// Adapter — implements the Domain port using Azure Blob Storage.
/// </summary>
internal sealed class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _container;

    public AzureBlobStorageService(IOptions<BlobStorageOptions> options)
    {
        var o = options.Value;
        _container = new BlobContainerClient(o.ConnectionString, o.ContainerName);
    }

    public async Task<string> UploadAsync(string folder, Stream stream, string contentType, CancellationToken ct = default)
    {
        var blobName = $"{folder}/{Guid.NewGuid()}{GetExtension(contentType)}";
        var blob = _container.GetBlobClient(blobName);

        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string blobUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(blobUrl))
            return;

        // Extract blob name from full URL
        if (Uri.TryCreate(blobUrl, UriKind.Absolute, out var uri))
        {
            // Path = /container/folder/file.ext → skip first two segments
            var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
            if (segments.Length == 2)
            {
                var blobName = segments[1];
                var blob = _container.GetBlobClient(blobName);
                await blob.DeleteIfExistsAsync(cancellationToken: ct);
            }
        }
    }

    private static string GetExtension(string contentType) => contentType switch
    {
        "image/webp" => ".webp",
        "image/png" => ".png",
        "image/jpeg" => ".jpg",
        "image/gif" => ".gif",
        "image/svg+xml" => ".svg",
        _ => ".bin"
    };
}
