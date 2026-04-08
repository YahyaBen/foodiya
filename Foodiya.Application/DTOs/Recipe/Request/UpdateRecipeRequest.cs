using System.ComponentModel.DataAnnotations;
using Foodiya.Domain.Constants;

namespace Foodiya.Application.DTOs.Recipe.Request;

public sealed class UpdateRecipeRequest
{
    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Summary { get; set; }

    public int? DifficultyId { get; set; }

    public int? CuisineId { get; set; }

    public int? CityId { get; set; }

    [Range(0, 1440)]
    public int? PrepTimeMinutes { get; set; }

    [Range(0, 1440)]
    public int? CookTimeMinutes { get; set; }

    [Range(1, 100)]
    public int? Servings { get; set; }

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    [StringLength(20)]
    [RegularExpression(RecipeVisibilityConstants.ValidationPattern)]
    public string? Visibility { get; set; }

    [RegularExpression(RecipeStatusConstants.ValidationPattern)]
    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public ICollection<int>? FoodCategoryIds { get; set; }
    public ICollection<CreateRecipeStepRequest>? Steps { get; set; }
    public ICollection<CreateRecipeIngredientItem>? Ingredients { get; set; }
}
