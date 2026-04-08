using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.MoroccanRegion.Request;
using Foodiya.Application.DTOs.MoroccanRegion.Response;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class MoroccanRegionController : BaseController
{
    private readonly IMoroccanRegionService _moroccanRegionService;

    public MoroccanRegionController(IMoroccanRegionService moroccanRegionService)
    {
        _moroccanRegionService = moroccanRegionService ?? throw new FoodiyaNullArgumentException(nameof(moroccanRegionService));
    }

    /// <summary>
    /// Get all Moroccan regions
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of Moroccan regions with optional filters by active status or free-text search.
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 12)</param>
    /// <param name="isActive">Filter by active status (optional)</param>
    /// <param name="search">Search on region name or code (optional)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<MoroccanRegionDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<MoroccanRegionDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _moroccanRegionService.ListAsync(page, pageSize, isActive, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a Moroccan region by ID
    /// </summary>
    /// <remarks>
    /// Returns the full Moroccan region details.
    /// </remarks>
    /// <param name="id">Moroccan region identifier</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MoroccanRegionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MoroccanRegionDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var region = await _moroccanRegionService.GetByIdAsync(id, ct);
        return region is null ? NotFound() : Ok(region);
    }

    /// <summary>
    /// Create a new Moroccan region
    /// </summary>
    /// <remarks>
    /// Creates a new Moroccan region entry. Code and name should be unique.
    /// </remarks>
    /// <param name="request">Moroccan region payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPost]
    [ProducesResponseType(typeof(MoroccanRegionDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MoroccanRegionDetailResponse>> Create([FromBody] CreateMoroccanRegionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _moroccanRegionService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing Moroccan region
    /// </summary>
    /// <remarks>
    /// Updates the editable Moroccan region fields.
    /// </remarks>
    /// <param name="id">Moroccan region identifier</param>
    /// <param name="request">Updated Moroccan region payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(MoroccanRegionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MoroccanRegionDetailResponse>> Update(int id, [FromBody] UpdateMoroccanRegionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _moroccanRegionService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Toggle Moroccan region active status
    /// </summary>
    /// <remarks>
    /// Flips the IsActive flag for the target Moroccan region.
    /// </remarks>
    /// <param name="id">Moroccan region identifier</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}/toggle-active")]
    [ProducesResponseType(typeof(MoroccanRegionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MoroccanRegionDetailResponse>> ToggleActive(int id, CancellationToken ct)
    {
        var region = await _moroccanRegionService.ToggleActiveAsync(id, ct);
        return Ok(region);
    }

    /// <summary>
    /// Delete a Moroccan region
    /// </summary>
    /// <remarks>
    /// Deletes the region when it is not referenced by Moroccan cities.
    /// </remarks>
    /// <param name="id">Moroccan region identifier</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        await _moroccanRegionService.DeleteAsync(id, ct);
        return NoContent();
    }
}
