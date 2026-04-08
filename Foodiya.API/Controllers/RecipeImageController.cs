using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeImage.Request;
using Foodiya.Application.DTOs.RecipeImage.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class RecipeImageController : BaseController
{
    private readonly IRecipeImageService _recipeImageService;

    public RecipeImageController(IRecipeImageService recipeImageService)
    {
        _recipeImageService = recipeImageService ?? throw new FoodiyaNullArgumentException(nameof(recipeImageService));
    }

    /// <summary>
    /// Get all recipe images
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of recipe images with optional filters by recipe, primary flag, or free-text search.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<RecipeImageDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<RecipeImageDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int? recipeId = null,
        [FromQuery] bool? isPrimary = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _recipeImageService.ListAsync(page, pageSize, recipeId, isPrimary, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a recipe image by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecipeImageDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeImageDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var recipeImage = await _recipeImageService.GetByIdAsync(id, ct);
        return recipeImage is null ? NotFound() : Ok(recipeImage);
    }

    /// <summary>
    /// Create a recipe image
    /// </summary>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(RecipeImageDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeImageDetailResponse>> Create([FromBody] CreateRecipeImageRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _recipeImageService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a recipe image
    /// </summary>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(RecipeImageDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeImageDetailResponse>> Update(int id, [FromBody] UpdateRecipeImageRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _recipeImageService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Delete a recipe image
    /// </summary>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        await _recipeImageService.DeleteAsync(id, ct);
        return NoContent();
    }
}
