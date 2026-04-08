using AutoMapper;
using Foodiya.Application.DTOs.Common;
using Foodiya.Application.DTOs.Recipe.Request;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Factories;
using Foodiya.Application.Interfaces.Services;
using Foodiya.Domain.Interfaces.Core;
using Foodiya.Domain.Models;
using Foodiya.Domain.Specifications.Recipes;

namespace Foodiya.Application.Services;

public sealed class RecipeService : IRecipeService
{
    private readonly IGenericRepository<Recipe> _recipeRepo;
    private readonly IGenericRepository<FoodCategory> _categoryRepo;
    private readonly IGenericRepository<Ingredient> _ingredientRepo;
    private readonly IMapper _mapper;
    private readonly IRecipeFactory _factory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RecipeService(
        IGenericRepository<Recipe> recipeRepo,
        IGenericRepository<FoodCategory> categoryRepo,
        IGenericRepository<Ingredient> ingredientRepo,
        IMapper mapper,
        IRecipeFactory factory,
        IDateTimeProvider dateTimeProvider)
    {
        _recipeRepo = recipeRepo;
        _categoryRepo = categoryRepo;
        _ingredientRepo = ingredientRepo;
        _mapper = mapper;
        _factory = factory;
        _dateTimeProvider = dateTimeProvider;
    }

    // ─── Queries ────────────────────────────────────────────────

    public async Task<RecipeDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var recipe = await _recipeRepo.GetSingleAsync(new RecipeByIdSpecification(id), ct);
        return recipe is null ? null : _mapper.Map<RecipeDetailResponse>(recipe);
    }

    public async Task<PaginatedResponse<RecipeDetailResponse>> ListAsync(
        int page, int pageSize,
        int? cuisineId, int? difficultyId, int? categoryId, string? search,
        CancellationToken ct = default)
    {
        var listSpec = new RecipeListSpecification(page, pageSize, cuisineId, difficultyId, categoryId, search);
        var countSpec = new RecipeCountSpecification(cuisineId, difficultyId, categoryId, search);

        var items = await _recipeRepo.ListAsync(listSpec, ct);
        var totalCount = await _recipeRepo.CountAsync(countSpec, ct);

        return new PaginatedResponse<RecipeDetailResponse>
        {
            Data = _mapper.Map<IReadOnlyList<RecipeDetailResponse>>(items),
            Meta = new PaginationMeta
            {
                Page = page,
                Take = pageSize,
                ItemCount = totalCount
            }
        };
    }

    // ─── Commands ───────────────────────────────────────────────

    public async Task<RecipeDetailResponse> CreateAsync(int chefId, CreateRecipeRequest request, CancellationToken ct = default)
    {
        var recipe = _factory.Create(chefId, request);

        await ResolveCategoriesAsync(recipe, request.FoodCategoryIds, ct);
        await ResolveIngredientsAsync(recipe, ct);

        await _recipeRepo.InsertAsync(recipe, ct);
        await _recipeRepo.SaveAsync(ct);

        return (await GetByIdInternalAsync(recipe.Id, ct))!;
    }

    public async Task<RecipeDetailResponse> UpdateAsync(int id, UpdateRecipeRequest request, CancellationToken ct = default)
    {
        var recipe = await _recipeRepo.GetSingleAsync(new RecipeForUpdateSpecification(id), ct)
            ?? throw new FoodiyaNotFoundException($"Recipe with ID {id} not found.");

        _factory.Update(recipe, request);

        if (request.FoodCategoryIds is not null)
            await ResolveCategoriesAsync(recipe, request.FoodCategoryIds, ct);

        if (request.Ingredients is not null)
            await ResolveIngredientsAsync(recipe, ct);

        _recipeRepo.Update(recipe);
        await _recipeRepo.SaveAsync(ct);

        return (await GetByIdInternalAsync(recipe.Id, ct))!;
    }

    public async Task<RecipeDetailResponse> ToggleActiveAsync(int id, CancellationToken ct = default)
    {
        var recipe = await _recipeRepo.GetByIdAsync(id, ct: ct)
            ?? throw new FoodiyaNotFoundException($"Recipe with ID {id} not found.");

        recipe.IsActive = !recipe.IsActive;
        recipe.DateModif = _dateTimeProvider.UtcNow;

        _recipeRepo.Update(recipe);
        await _recipeRepo.SaveAsync(ct);

        return (await GetByIdInternalAsync(recipe.Id, ct))!;
    }

    public async Task DeleteAsync(int id, SoftDeleteRequest request, CancellationToken ct = default)
    {
        var recipe = await _recipeRepo.GetByIdAsync(id, ct: ct)
            ?? throw new FoodiyaNotFoundException($"Recipe with ID {id} not found.");

        if (recipe.DeletedAt is not null)
            throw new FoodiyaBadRequestException($"Recipe with ID {id} is already deleted.");

        recipe.DeletedAt = _dateTimeProvider.UtcNow;
        recipe.DeletedByUserId = request.DeletedByUserId;
        recipe.DeleteReason = request.DeleteReason;
        recipe.DateModif = _dateTimeProvider.UtcNow;

        _recipeRepo.Update(recipe);
        await _recipeRepo.SaveAsync(ct);
    }

    // ─── Private helpers ────────────────────────────────────────

    private async Task<RecipeDetailResponse?> GetByIdInternalAsync(int id, CancellationToken ct)
    {
        var recipe = await _recipeRepo.GetSingleAsync(new RecipeByIdSpecification(id, includeInactive: true), ct);
        return recipe is null ? null : _mapper.Map<RecipeDetailResponse>(recipe);
    }

    private async Task ResolveCategoriesAsync(Recipe recipe, ICollection<int> categoryIds, CancellationToken ct)
    {
        recipe.FoodCategories.Clear();
        foreach (var catId in categoryIds)
        {
            var category = await _categoryRepo.GetByIdAsync(catId, ct: ct)
                ?? throw new FoodiyaNotFoundException($"FoodCategory with ID {catId} not found.");
            recipe.FoodCategories.Add(category);
        }
    }

    /// <summary>
    /// Resolves each RecipeIngredient's Ingredient navigation from the DB
    /// so EF's change tracker can properly handle the composite PK/FK.
    /// </summary>
    private async Task ResolveIngredientsAsync(Recipe recipe, CancellationToken ct)
    {
        foreach (var ri in recipe.RecipeIngredients)
        {
            ri.Ingredient = await _ingredientRepo.GetByIdAsync(ri.IngredientId, ct: ct)
                ?? throw new FoodiyaNotFoundException($"Ingredient with ID {ri.IngredientId} not found.");
        }
    }
}
