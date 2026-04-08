using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeIngredient.Request;
using Foodiya.Application.DTOs.RecipeIngredient.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class RecipeIngredientController : BaseController
{
    private readonly IRecipeIngredientService _recipeIngredientService;

    public RecipeIngredientController(IRecipeIngredientService recipeIngredientService)
    {
        _recipeIngredientService = recipeIngredientService ?? throw new FoodiyaNullArgumentException(nameof(recipeIngredientService));
    }

    /// <summary>
    /// Get all recipe ingredients
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of recipe ingredients with optional filters by recipe, ingredient, unit, or free-text search.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<RecipeIngredientDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<RecipeIngredientDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int? recipeId = null,
        [FromQuery] int? ingredientId = null,
        [FromQuery] int? unitId = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _recipeIngredientService.ListAsync(page, pageSize, recipeId, ingredientId, unitId, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a recipe ingredient by composite key
    /// </summary>
    [HttpGet("{recipeId:int}/{ingredientId:int}")]
    [ProducesResponseType(typeof(RecipeIngredientDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeIngredientDetailResponse>> GetById(int recipeId, int ingredientId, CancellationToken ct)
    {
        var recipeIngredient = await _recipeIngredientService.GetByIdAsync(recipeId, ingredientId, ct);
        return recipeIngredient is null ? NotFound() : Ok(recipeIngredient);
    }

    /// <summary>
    /// Create a recipe ingredient
    /// </summary>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(RecipeIngredientDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecipeIngredientDetailResponse>> Create([FromBody] CreateRecipeIngredientRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _recipeIngredientService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { recipeId = created.RecipeId, ingredientId = created.IngredientId }, created);
    }

    /// <summary>
    /// Update a recipe ingredient
    /// </summary>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPatch("{recipeId:int}/{ingredientId:int}")]
    [ProducesResponseType(typeof(RecipeIngredientDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeIngredientDetailResponse>> Update(
        int recipeId,
        int ingredientId,
        [FromBody] UpdateRecipeIngredientRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _recipeIngredientService.UpdateAsync(recipeId, ingredientId, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Delete a recipe ingredient
    /// </summary>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpDelete("{recipeId:int}/{ingredientId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int recipeId, int ingredientId, CancellationToken ct)
    {
        await _recipeIngredientService.DeleteAsync(recipeId, ingredientId, ct);
        return NoContent();
    }
}
