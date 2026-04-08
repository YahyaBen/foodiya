using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.MoroccanCity.Request;

public sealed class CreateMoroccanCityRequest
{
    [Required]
    public int RegionId { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
