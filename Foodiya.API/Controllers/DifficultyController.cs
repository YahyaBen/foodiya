using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.Difficulty.Request;
using Foodiya.Application.DTOs.Difficulty.Response;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class DifficultyController : BaseController
{
    private readonly IDifficultyService _difficultyService;

    public DifficultyController(IDifficultyService difficultyService)
    {
        _difficultyService = difficultyService ?? throw new FoodiyaNullArgumentException(nameof(difficultyService));
    }

    /// <summary>
    /// Get all difficulties
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of difficulties with optional filters by active status or free-text search.
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 12)</param>
    /// <param name="isActive">Filter by active status (optional)</param>
    /// <param name="search">Search on difficulty name or code (optional)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<DifficultyDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<DifficultyDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _difficultyService.ListAsync(page, pageSize, isActive, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a difficulty by ID
    /// </summary>
    /// <remarks>
    /// Returns the full difficulty details.
    /// </remarks>
    /// <param name="id">Difficulty identifier</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DifficultyDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DifficultyDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var difficulty = await _difficultyService.GetByIdAsync(id, ct);
        return difficulty is null ? NotFound() : Ok(difficulty);
    }

    /// <summary>
    /// Create a new difficulty
    /// </summary>
    /// <remarks>
    /// Creates a new difficulty entry. Name and code should be unique.
    /// </remarks>
    /// <param name="request">Difficulty payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(DifficultyDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DifficultyDetailResponse>> Create([FromBody] CreateDifficultyRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _difficultyService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing difficulty
    /// </summary>
    /// <remarks>
    /// Updates the editable difficulty fields. Optional text fields can be cleared by sending an empty string.
    /// </remarks>
    /// <param name="id">Difficulty identifier</param>
    /// <param name="request">Updated difficulty payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(DifficultyDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DifficultyDetailResponse>> Update(int id, [FromBody] UpdateDifficultyRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _difficultyService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Toggle difficulty active status
    /// </summary>
    /// <remarks>
    /// Flips the IsActive flag for the target difficulty.
    /// </remarks>
    /// <param name="id">Difficulty identifier</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}/toggle-active")]
    [ProducesResponseType(typeof(DifficultyDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DifficultyDetailResponse>> ToggleActive(int id, CancellationToken ct)
    {
        var difficulty = await _difficultyService.ToggleActiveAsync(id, ct);
        return Ok(difficulty);
    }

    /// <summary>
    /// Delete a difficulty
    /// </summary>
    /// <remarks>
    /// Deletes the difficulty when it is not referenced by recipes.
    /// </remarks>
    /// <param name="id">Difficulty identifier</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        await _difficultyService.DeleteAsync(id, ct);
        return NoContent();
    }
}
