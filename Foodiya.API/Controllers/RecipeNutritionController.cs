using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeNutrition.Request;
using Foodiya.Application.DTOs.RecipeNutrition.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class RecipeNutritionController : BaseController
{
    private readonly IRecipeNutritionService _recipeNutritionService;

    public RecipeNutritionController(IRecipeNutritionService recipeNutritionService)
    {
        _recipeNutritionService = recipeNutritionService ?? throw new FoodiyaNullArgumentException(nameof(recipeNutritionService));
    }

    /// <summary>
    /// Get all recipe nutrition rows
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of recipe nutrition records with optional filters by recipe or free-text search on recipe title.
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 12)</param>
    /// <param name="recipeId">Filter by recipe identifier (optional)</param>
    /// <param name="search">Search on recipe title (optional)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<RecipeNutritionDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<RecipeNutritionDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int? recipeId = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _recipeNutritionService.ListAsync(page, pageSize, recipeId, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get recipe nutrition by recipe ID
    /// </summary>
    /// <remarks>
    /// Returns the nutrition row linked to the specified recipe.
    /// </remarks>
    /// <param name="recipeId">Recipe identifier</param>
    [HttpGet("{recipeId:int}")]
    [ProducesResponseType(typeof(RecipeNutritionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeNutritionDetailResponse>> GetById(int recipeId, CancellationToken ct)
    {
        var recipeNutrition = await _recipeNutritionService.GetByIdAsync(recipeId, ct);
        return recipeNutrition is null ? NotFound() : Ok(recipeNutrition);
    }

    /// <summary>
    /// Create recipe nutrition
    /// </summary>
    /// <remarks>
    /// Creates the one-to-one nutrition row for a recipe. A recipe can only have one nutrition record.
    /// </remarks>
    /// <param name="request">Recipe nutrition payload</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(RecipeNutritionDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecipeNutritionDetailResponse>> Create([FromBody] CreateRecipeNutritionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _recipeNutritionService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { recipeId = created.RecipeId }, created);
    }

    /// <summary>
    /// Update recipe nutrition
    /// </summary>
    /// <remarks>
    /// Updates the nutrition values for the target recipe. Nullable macro fields can be cleared with the explicit clear flags.
    /// </remarks>
    /// <param name="recipeId">Recipe identifier</param>
    /// <param name="request">Updated recipe nutrition payload</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPatch("{recipeId:int}")]
    [ProducesResponseType(typeof(RecipeNutritionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeNutritionDetailResponse>> Update(int recipeId, [FromBody] UpdateRecipeNutritionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _recipeNutritionService.UpdateAsync(recipeId, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Delete recipe nutrition
    /// </summary>
    /// <remarks>
    /// Deletes the nutrition row linked to the specified recipe.
    /// </remarks>
    /// <param name="recipeId">Recipe identifier</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpDelete("{recipeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int recipeId, CancellationToken ct)
    {
        await _recipeNutritionService.DeleteAsync(recipeId, ct);
        return NoContent();
    }
}
