using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.RecipeIngredient.Request;

public sealed class CreateRecipeIngredientRequest
{
    [Required]
    public int RecipeId { get; set; }

    [Required]
    public int IngredientId { get; set; }

    [Range(0.01, 99999)]
    public decimal Quantity { get; set; }

    public int? UnitId { get; set; }

    public bool IsOptional { get; set; }

    [StringLength(250)]
    public string? Notes { get; set; }

    [Range(0, 100)]
    public int SortOrder { get; set; }
}
