namespace Foodiya.Application.DTOs.RecipeShare.Response;

public sealed class RecipeShareDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int RecipeId { get; set; }
    public string RecipeTitle { get; set; } = string.Empty;
    public int SharedByUserId { get; set; }
    public string SharedByUserName { get; set; } = string.Empty;
    public string SharedByUserFullName { get; set; } = string.Empty;
    public int? SharedWithUserId { get; set; }
    public string? SharedWithUserName { get; set; }
    public string? SharedWithUserFullName { get; set; }
    public string ShareChannel { get; set; } = string.Empty;
    public string? ShareMessage { get; set; }
    public DateTime SharedAt { get; set; }
}
