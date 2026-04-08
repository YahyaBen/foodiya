using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.RecipeImage.Request;

public sealed class CreateRecipeImageRequest
{
    [Required]
    public int RecipeId { get; set; }

    [Required, StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(250)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }
}
