using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.Ingredient.Request;

public sealed class UpdateIngredientRequest
{
    [StringLength(150)]
    public string? Name { get; set; }

    public int? IngredientTypeId { get; set; }

    public int? DefaultUnitId { get; set; }

    public bool ClearDefaultUnit { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal? CaloriesPer100g { get; set; }

    public bool ClearCaloriesPer100g { get; set; }

    public bool? IsActive { get; set; }
}
