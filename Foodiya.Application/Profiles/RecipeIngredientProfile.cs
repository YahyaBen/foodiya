using AutoMapper;
using Foodiya.Application.DTOs.RecipeIngredient.Response;
using Foodiya.Domain.Models;

namespace Foodiya.Application.Profiles;

public sealed class RecipeIngredientProfile : Profile
{
    public RecipeIngredientProfile()
    {
        CreateMap<RecipeIngredient, RecipeIngredientDetailResponse>()
            .ForMember(d => d.RecipeTitle, o => o.MapFrom(s => s.Recipe.Title))
            .ForMember(d => d.IngredientName, o => o.MapFrom(s => s.Ingredient.Name))
            .ForMember(d => d.UnitLabel, o => o.MapFrom(s => s.Unit != null ? s.Unit.Label : null));
    }
}
