namespace Foodiya.Application.DTOs.RecipeLike.Response;

public sealed class RecipeLikeDetailResponse
{
    public int RecipeId { get; set; }
    public string RecipeTitle { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public DateTime LikedAt { get; set; }
}
