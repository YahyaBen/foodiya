using Foodiya.Application.DTOs.MoroccanRegion.Request;
using Foodiya.Application.Interfaces.Factories;
using Foodiya.Domain.Extensions;
using Foodiya.Domain.Interfaces.Core;
using Foodiya.Domain.Models;
using static Foodiya.Application.Factories.Helpers.EntityNormalizationHelper;

namespace Foodiya.Application.Factories;

public sealed class MoroccanRegionFactory : IMoroccanRegionFactory
{
    public MoroccanRegion Create(CreateMoroccanRegionRequest request) => new()
    {
        Code = EntityCodeGenerator.For("REG"),
        Name = Required(request.Name, nameof(request.Name)),
        SortOrder = request.SortOrder,
        IsActive = request.IsActive
    };

    public void Update(MoroccanRegion region, UpdateMoroccanRegionRequest request, DateTime utcNow)
    {
        if (request.Name is not null)
            region.Name = Required(request.Name, nameof(request.Name));

        if (request.SortOrder.HasValue)
            region.SortOrder = request.SortOrder.Value;

        if (request.IsActive.HasValue)
            region.IsActive = request.IsActive.Value;

        region.DateModif = utcNow;
    }
}
