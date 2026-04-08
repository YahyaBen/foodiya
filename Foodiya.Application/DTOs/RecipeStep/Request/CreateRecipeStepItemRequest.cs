using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.RecipeStep.Request;

public sealed class CreateRecipeStepItemRequest
{
    [Required]
    public int RecipeId { get; set; }

    [Range(1, 100)]
    public int StepNumber { get; set; }

    [StringLength(150)]
    public string? Title { get; set; }

    [Required, StringLength(2000)]
    public string Instruction { get; set; } = string.Empty;

    [Range(0, 1440)]
    public int? DurationMinutes { get; set; }
}
