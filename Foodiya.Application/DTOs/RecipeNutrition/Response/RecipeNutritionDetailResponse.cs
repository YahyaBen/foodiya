namespace Foodiya.Application.DTOs.RecipeNutrition.Response;

public sealed class RecipeNutritionDetailResponse
{
    public int RecipeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string RecipeTitle { get; set; } = string.Empty;
    public decimal CaloriesPerServing { get; set; }
    public decimal? ProteinGrams { get; set; }
    public decimal? CarbsGrams { get; set; }
    public decimal? FatGrams { get; set; }
}
