using System.ComponentModel.DataAnnotations;

namespace Foodiya.Application.DTOs.MoroccanCity.Request;

public sealed class UpdateMoroccanCityRequest
{
    public int? RegionId { get; set; }

    [StringLength(150)]
    public string? Name { get; set; }

    [Range(0, int.MaxValue)]
    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }
}
