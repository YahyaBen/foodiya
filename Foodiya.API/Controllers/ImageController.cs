using Foodiya.API.Controllers.Common;
using Foodiya.Application.DTOs.Image;
using Foodiya.Application.Interfaces.Services;
using Foodiya.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class ImageController : BaseController
{
    private readonly IImageUploadService _imageUploadService;

    public ImageController(IImageUploadService imageUploadService)
    {
        _imageUploadService = imageUploadService;
    }

    /// <summary>
    /// Upload an image
    /// </summary>
    /// <remarks>
    /// Uploads a file to the specified folder and returns its public URL.
    /// Use the returned URL in create/update payloads (e.g. cuisine IconUrl, recipe CoverImageUrl, etc.).
    ///
    /// Allowed folders: users, chefs, recipes, ingredients, cuisines, difficulties, cities, regions, avatars.
    /// Max file size: 2 MB. Allowed types: jpeg, png, webp, gif, svg.
    /// </remarks>
    /// <param name="folder">Target folder (e.g. "cuisines")</param>
    /// <param name="file">The image file</param>
    [HttpPost("upload")]
    [Authorize(Roles = AppRoleConstants.All)]
    [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<ActionResult<ImageUploadResponse>> Upload(
        [FromQuery] string folder,
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        using var stream = file.OpenReadStream();
        var result = await _imageUploadService.UploadAsync(folder, stream, file.ContentType, file.Length, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete an image
    /// </summary>
    /// <remarks>
    /// Deletes a previously uploaded image by its full URL. No-op if the image does not exist.
    /// </remarks>
    /// <param name="imageUrl">Full public URL of the image to delete</param>
    [HttpDelete]
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromQuery] string imageUrl, CancellationToken ct)
    {
        await _imageUploadService.DeleteAsync(imageUrl, ct);
        return NoContent();
    }
}
