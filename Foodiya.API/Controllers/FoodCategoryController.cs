using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.FoodCategory.Request;
using Foodiya.Application.DTOs.FoodCategory.Response;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class FoodCategoryController : BaseController
{
    private readonly IFoodCategoryService _foodCategoryService;

    public FoodCategoryController(IFoodCategoryService foodCategoryService)
    {
        _foodCategoryService = foodCategoryService ?? throw new FoodiyaNullArgumentException(nameof(foodCategoryService));
    }

    /// <summary>
    /// Get all food categories
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of food categories with optional filters by active status or free-text search.
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 12)</param>
    /// <param name="isActive">Filter by active status (optional)</param>
    /// <param name="search">Search on category name, description, or code (optional)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<FoodCategoryDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<FoodCategoryDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _foodCategoryService.ListAsync(page, pageSize, isActive, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a food category by ID
    /// </summary>
    /// <remarks>
    /// Returns the full food category details.
    /// </remarks>
    /// <param name="id">Food category identifier</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FoodCategoryDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodCategoryDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var foodCategory = await _foodCategoryService.GetByIdAsync(id, ct);
        return foodCategory is null ? NotFound() : Ok(foodCategory);
    }

    /// <summary>
    /// Create a new food category
    /// </summary>
    /// <remarks>
    /// Creates a new food category entry. Name and code should be unique.
    /// </remarks>
    /// <param name="request">Food category payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(FoodCategoryDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FoodCategoryDetailResponse>> Create([FromBody] CreateFoodCategoryRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _foodCategoryService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing food category
    /// </summary>
    /// <remarks>
    /// Updates the editable food category fields. Optional text fields can be cleared by sending an empty string.
    /// </remarks>
    /// <param name="id">Food category identifier</param>
    /// <param name="request">Updated food category payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(FoodCategoryDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FoodCategoryDetailResponse>> Update(int id, [FromBody] UpdateFoodCategoryRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _foodCategoryService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Toggle food category active status
    /// </summary>
    /// <remarks>
    /// Flips the IsActive flag for the target food category.
    /// </remarks>
    /// <param name="id">Food category identifier</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}/toggle-active")]
    [ProducesResponseType(typeof(FoodCategoryDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodCategoryDetailResponse>> ToggleActive(int id, CancellationToken ct)
    {
        var foodCategory = await _foodCategoryService.ToggleActiveAsync(id, ct);
        return Ok(foodCategory);
    }

    /// <summary>
    /// Delete a food category
    /// </summary>
    /// <remarks>
    /// Deletes the food category when it is not referenced by recipes.
    /// </remarks>
    /// <param name="id">Food category identifier</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        await _foodCategoryService.DeleteAsync(id, ct);
        return NoContent();
    }
}
