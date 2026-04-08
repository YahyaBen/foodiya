using System.ComponentModel.DataAnnotations;
using Foodiya.Domain.Constants;

namespace Foodiya.Application.DTOs.RecipeShare.Request;

public sealed class CreateRecipeShareRequest
{
    [Required]
    public int RecipeId { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    public int? SharedWithUserId { get; set; }

    [Required, StringLength(30)]
    [RegularExpression(ShareChannelConstants.ValidationPattern)]
    public string ShareChannel { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ShareMessage { get; set; }
}
