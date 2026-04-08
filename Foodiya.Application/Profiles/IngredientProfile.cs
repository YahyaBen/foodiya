using AutoMapper;
using Foodiya.Application.DTOs.Ingredient.Response;
using Foodiya.Domain.Models;

namespace Foodiya.Application.Profiles;

public sealed class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDetailResponse>()
            .ForMember(d => d.IngredientTypeLabel, o => o.MapFrom(s => s.IngredientType.Label))
            .ForMember(d => d.DefaultUnitLabel, o => o.MapFrom(s => s.DefaultUnit != null ? s.DefaultUnit.Label : null));
    }
}
