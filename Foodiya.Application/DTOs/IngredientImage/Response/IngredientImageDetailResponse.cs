namespace Foodiya.Application.DTOs.IngredientImage.Response;

public sealed class IngredientImageDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
