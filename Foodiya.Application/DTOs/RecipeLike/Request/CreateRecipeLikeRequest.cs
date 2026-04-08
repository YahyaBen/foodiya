using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.RecipeLike.Request;

public sealed class CreateRecipeLikeRequest
{
    [Required]
    public int RecipeId { get; set; }

    [Required]
    public int UserId { get; set; }
}
