using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.Ingredient.Request;

public sealed class CreateIngredientRequest
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int IngredientTypeId { get; set; }

    public int? DefaultUnitId { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal? CaloriesPer100g { get; set; }

    public bool IsActive { get; set; } = true;
}
