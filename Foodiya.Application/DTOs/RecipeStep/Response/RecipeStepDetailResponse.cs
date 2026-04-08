namespace Foodiya.Application.DTOs.RecipeStep.Response;

public sealed class RecipeStepDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int RecipeId { get; set; }
    public string RecipeTitle { get; set; } = string.Empty;
    public int StepNumber { get; set; }
    public string? Title { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public int? DurationMinutes { get; set; }
}
