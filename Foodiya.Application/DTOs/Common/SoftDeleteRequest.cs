using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.Common;

/// <summary>
/// Payload required when soft-deleting an entity.
/// Forces the caller to provide a reason and identify themselves.
/// </summary>
public sealed class SoftDeleteRequest
{
    /// <summary>
    /// The ID of the user performing the deletion.
    /// </summary>
    [Required]
    public int DeletedByUserId { get; set; }

    /// <summary>
    /// Mandatory reason explaining why the entity is being deleted.
    /// </summary>
    [Required, StringLength(500, MinimumLength = 3)]
    public string DeleteReason { get; set; } = string.Empty;
}
