using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.ChefProfile.Request;
using Foodiya.Application.DTOs.ChefProfile.Response;
using Foodiya.Application.DTOs.Common;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class ChefProfileController : BaseController
{
    private readonly IChefProfileService _chefProfileService;

    public ChefProfileController(IChefProfileService chefProfileService)
    {
        _chefProfileService = chefProfileService ?? throw new FoodiyaNullArgumentException(nameof(chefProfileService));
    }

    /// <summary>
    /// Get all chef profiles
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of chef profiles with optional filters by verification status or free-text search.
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 12)</param>
    /// <param name="isVerified">Filter by verification status (optional)</param>
    /// <param name="search">Search on display name, bio, specialty, or user identity fields (optional)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ChefProfileDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<ChefProfileDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] bool? isVerified = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _chefProfileService.ListAsync(page, pageSize, isVerified, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a chef profile by ID
    /// </summary>
    /// <remarks>
    /// Returns the chef profile details together with the linked AppUser identity fields.
    /// </remarks>
    /// <param name="id">Chef profile identifier</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChefProfileDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefProfileDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var chefProfile = await _chefProfileService.GetByIdAsync(id, ct);
        return chefProfile is null ? NotFound() : Ok(chefProfile);
    }

    /// <summary>
    /// Create my chef profile (self-registration)
    /// </summary>
    /// <remarks>
    /// Any authenticated user can create their own chef profile.
    /// The owner is derived from the JWT token – no user ID needs to be supplied.
    /// One user can own only one chef profile.
    /// </remarks>
    /// <param name="request">Chef profile payload</param>
    [HttpPost("me")]
    [ProducesResponseType(typeof(ChefProfileDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChefProfileDetailResponse>> CreateMine([FromBody] CreateChefProfileRequest request, CancellationToken ct)
    {
        var subClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(subClaim, out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _chefProfileService.CreateAsync(userId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Create a chef profile for another user (admin)
    /// </summary>
    /// <remarks>
    /// Admin-only endpoint. Creates a chef profile on behalf of the specified AppUser.
    /// One user can own only one chef profile.
    /// </remarks>
    /// <param name="userId">AppUser identifier that will own the profile</param>
    /// <param name="request">Chef profile payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPost("{userId:int}")]
    [ProducesResponseType(typeof(ChefProfileDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChefProfileDetailResponse>> Create(int userId, [FromBody] CreateChefProfileRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _chefProfileService.CreateAsync(userId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing chef profile
    /// </summary>
    /// <remarks>
    /// Updates the editable chef profile fields. Optional text fields can be cleared by sending an empty string.
    /// </remarks>
    /// <param name="id">Chef profile identifier</param>
    /// <param name="request">Updated chef profile payload</param>
    [Authorize(Roles = AppRoleConstants.AdminOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ChefProfileDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefProfileDetailResponse>> Update(int id, [FromBody] UpdateChefProfileRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _chefProfileService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Toggle chef verification status
    /// </summary>
    /// <remarks>
    /// Flips the IsVerified flag for the target chef profile.
    /// </remarks>
    /// <param name="id">Chef profile identifier</param>
    [Authorize(Roles = AppRoleConstants.SuperAdminOnly)]
    [HttpPatch("{id:int}/toggle-verified")]
    [ProducesResponseType(typeof(ChefProfileDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefProfileDetailResponse>> ToggleVerified(int id, CancellationToken ct)
    {
        var chefProfile = await _chefProfileService.ToggleVerifiedAsync(id, ct);
        return Ok(chefProfile);
    }

    /// <summary>
    /// Delete a chef profile (soft delete)
    /// </summary>
    /// <remarks>
    /// Marks the chef profile as deleted without removing data.
    /// Requires the user performing the action and a mandatory reason.
    /// Cannot delete a profile that still owns active recipes.
    /// </remarks>
    /// <param name="id">Chef profile identifier</param>
    /// <param name="request">User and reason for the deletion</param>
    [Authorize(Roles = AppRoleConstants.SuperAdminOnly)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, [FromBody] SoftDeleteRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _chefProfileService.DeleteAsync(id, request, ct);
        return NoContent();
    }
}
