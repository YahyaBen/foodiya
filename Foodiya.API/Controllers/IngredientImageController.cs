using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.IngredientImage.Request;
using Foodiya.Application.DTOs.IngredientImage.Response;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class IngredientImageController : BaseController
{
    private readonly IIngredientImageService _ingredientImageService;

    public IngredientImageController(IIngredientImageService ingredientImageService)
    {
        _ingredientImageService = ingredientImageService ?? throw new FoodiyaNullArgumentException(nameof(ingredientImageService));
    }

    /// <summary>
    /// Get all ingredient images
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of ingredient images with optional filters by ingredient, primary flag, or free-text search.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<IngredientImageDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<IngredientImageDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int? ingredientId = null,
        [FromQuery] bool? isPrimary = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _ingredientImageService.ListAsync(page, pageSize, ingredientId, isPrimary, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get an ingredient image by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(IngredientImageDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IngredientImageDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var ingredientImage = await _ingredientImageService.GetByIdAsync(id, ct);
        return ingredientImage is null ? NotFound() : Ok(ingredientImage);
    }

    /// <summary>
    /// Create an ingredient image
    /// </summary>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(IngredientImageDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IngredientImageDetailResponse>> Create([FromBody] CreateIngredientImageRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _ingredientImageService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an ingredient image
    /// </summary>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(IngredientImageDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IngredientImageDetailResponse>> Update(int id, [FromBody] UpdateIngredientImageRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _ingredientImageService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Delete an ingredient image
    /// </summary>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        await _ingredientImageService.DeleteAsync(id, ct);
        return NoContent();
    }
}
