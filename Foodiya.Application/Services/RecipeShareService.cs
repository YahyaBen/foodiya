using AutoMapper;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Application.DTOs.RecipeShare.Request;
using Foodiya.Application.DTOs.RecipeShare.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Factories;
using Foodiya.Application.Interfaces.Services;
using Foodiya.Domain.Interfaces.Core;
using Foodiya.Domain.Models;
using Foodiya.Domain.Specifications.RecipeShares;

namespace Foodiya.Application.Services;

public sealed class RecipeShareService : IRecipeShareService
{
    private readonly IRecipeShareRepository _recipeShareRepo;
    private readonly IGenericRepository<Recipe> _recipeRepo;
    private readonly IGenericRepository<AppUser> _userRepo;
    private readonly IMapper _mapper;
    private readonly IRecipeShareFactory _factory;

    public RecipeShareService(
        IRecipeShareRepository recipeShareRepo,
        IGenericRepository<Recipe> recipeRepo,
        IGenericRepository<AppUser> userRepo,
        IMapper mapper,
        IRecipeShareFactory factory)
    {
        _recipeShareRepo = recipeShareRepo;
        _recipeRepo = recipeRepo;
        _userRepo = userRepo;
        _mapper = mapper;
        _factory = factory;
    }

    public async Task<RecipeShareDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var recipeShare = await _recipeShareRepo.GetSingleAsync(new RecipeShareByIdSpecification(id), ct);
        return recipeShare is null ? null : _mapper.Map<RecipeShareDetailResponse>(recipeShare);
    }

    public async Task<PaginatedResponse<RecipeShareDetailResponse>> ListAsync(
        int page,
        int pageSize,
        int? recipeId,
        int? sharedByUserId,
        int? sharedWithUserId,
        string? channel,
        string? search,
        CancellationToken ct = default)
    {
        var listSpec = new RecipeShareListSpecification(page, pageSize, recipeId, sharedByUserId, sharedWithUserId, channel, search);
        var countSpec = new RecipeShareCountSpecification(recipeId, sharedByUserId, sharedWithUserId, channel, search);

        var items = await _recipeShareRepo.ListAsync(listSpec, ct);
        var totalCount = await _recipeShareRepo.CountAsync(countSpec, ct);

        return new PaginatedResponse<RecipeShareDetailResponse>
        {
            Data = _mapper.Map<IReadOnlyList<RecipeShareDetailResponse>>(items),
            Meta = new PaginationMeta
            {
                Page = page,
                Take = pageSize,
                ItemCount = totalCount
            }
        };
    }

    public async Task<RecipeShareDetailResponse> CreateAsync(CreateRecipeShareRequest request, CancellationToken ct = default)
    {
        await EnsureRecipeExistsAsync(request.RecipeId, ct);
        await EnsureUserExistsAsync(request.SharedByUserId, "SharedByUser", ct);

        if (request.SharedWithUserId.HasValue)
            await EnsureUserExistsAsync(request.SharedWithUserId.Value, "SharedWithUser", ct);

        var recipeShare = _factory.Create(request);

        await _recipeShareRepo.InsertAsync(recipeShare, ct);
        await _recipeShareRepo.SaveAsync(ct);

        return (await GetByIdInternalAsync(recipeShare.Id, ct))!;
    }

    public async Task<RecipeShareDetailResponse> UpdateAsync(int id, UpdateRecipeShareRequest request, CancellationToken ct = default)
    {
        var recipeShare = await _recipeShareRepo.GetByIdAsync(id, ct: ct)
            ?? throw new FoodiyaNotFoundException($"RecipeShare with ID {id} not found.");

        _factory.Update(recipeShare, request);

        if (!request.ClearSharedWithUser && request.SharedWithUserId.HasValue)
            await EnsureUserExistsAsync(request.SharedWithUserId.Value, "SharedWithUser", ct);

        _recipeShareRepo.Update(recipeShare);
        await _recipeShareRepo.SaveAsync(ct);

        return (await GetByIdInternalAsync(recipeShare.Id, ct))!;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var recipeShare = await _recipeShareRepo.GetByIdAsync(id, ct: ct)
            ?? throw new FoodiyaNotFoundException($"RecipeShare with ID {id} not found.");

        _recipeShareRepo.Delete(recipeShare);
        await _recipeShareRepo.SaveAsync(ct);
    }

    private async Task<RecipeShareDetailResponse?> GetByIdInternalAsync(int id, CancellationToken ct)
    {
        var recipeShare = await _recipeShareRepo.GetSingleAsync(new RecipeShareByIdSpecification(id), ct);
        return recipeShare is null ? null : _mapper.Map<RecipeShareDetailResponse>(recipeShare);
    }

    private async Task EnsureRecipeExistsAsync(int recipeId, CancellationToken ct)
    {
        var recipe = await _recipeRepo.GetByIdAsync(recipeId, ct: ct)
            ?? throw new FoodiyaNotFoundException($"Recipe with ID {recipeId} not found.");

        if (recipe.DeletedAt is not null)
            throw new FoodiyaBadRequestException($"Recipe with ID {recipeId} is deleted and cannot be shared.");
    }

    private async Task EnsureUserExistsAsync(int userId, string roleName, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct: ct)
            ?? throw new FoodiyaNotFoundException($"{roleName} AppUser with ID {userId} not found.");

        if (user.DeletedAt is not null)
            throw new FoodiyaBadRequestException($"{roleName} AppUser with ID {userId} is deleted and cannot be used in a share.");
    }
}
