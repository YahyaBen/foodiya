using Foodiya.API.Controllers.Common;
using Foodiya.Domain.Constants;
using Foodiya.Application.DTOs.Common;
using Foodiya.Application.DTOs.Recipe.Request;
using Foodiya.Application.DTOs.Recipe.Response;
using Foodiya.Domain.Exceptions;
using Foodiya.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foodiya.API.Controllers;

public sealed class RecipeController : BaseController
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService ?? throw new FoodiyaNullArgumentException(nameof(recipeService));
    }

    /// <summary>
    /// Récupérer toutes les recettes
    /// </summary>
    /// <remarks>
    /// Retourne une liste paginée de recettes publiées avec possibilité de filtrer par cuisine, difficulté, catégorie ou recherche textuelle.
    /// </remarks>
    /// <param name="page">Numéro de la page (défaut : 1)</param>
    /// <param name="pageSize">Nombre d'éléments par page (défaut : 12)</param>
    /// <param name="cuisineId">Filtrer par identifiant de cuisine (optionnel). Voir GET /api/Referential pour la liste.</param>
    /// <param name="difficultyId">Filtrer par identifiant de difficulté (optionnel). Voir GET /api/Referential pour la liste.</param>
    /// <param name="categoryId">Filtrer par identifiant de catégorie (optionnel). Voir GET /api/Referential pour la liste.</param>
    /// <param name="search">Recherche textuelle sur le titre ou le résumé (optionnel)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<RecipeDetailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<RecipeDetailResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int? cuisineId = null,
        [FromQuery] int? difficultyId = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _recipeService.ListAsync(page, pageSize, cuisineId, difficultyId, categoryId, search, ct);
        return Ok(result);
    }

    /// <summary>
    /// Récupérer une recette par son identifiant
    /// </summary>
    /// <remarks>
    /// Retourne le détail complet d'une recette (ingrédients, étapes, images, nutrition, etc.).
    /// </remarks>
    /// <param name="id">Identifiant de la recette</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecipeDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailResponse>> GetById(int id, CancellationToken ct)
    {
        var recipe = await _recipeService.GetByIdAsync(id, ct);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    /// <summary>
    /// Créer une nouvelle recette
    /// </summary>
    /// <remarks>
    /// Crée une recette pour le chef spécifié. Les catégories, ingrédients et étapes sont inclus dans le body.
    /// Le slug est généré automatiquement à partir du titre.
    /// </remarks>
    /// <param name="chefId">Identifiant du chef propriétaire</param>
    /// <param name="request">Données de la recette à créer</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPost("{chefId:int}")]
    [ProducesResponseType(typeof(RecipeDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDetailResponse>> Create(int chefId, [FromBody] CreateRecipeRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _recipeService.CreateAsync(chefId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Modifier une recette existante
    /// </summary>
    /// <remarks>
    /// Met à jour les champs de la recette ainsi que ses collections enfants (étapes, ingrédients, catégories).
    /// Les collections sont entièrement remplacées par celles fournies dans le body.
    /// </remarks>
    /// <param name="id">Identifiant de la recette à modifier</param>
    /// <param name="request">Données de mise à jour</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(RecipeDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailResponse>> Update(int id, [FromBody] UpdateRecipeRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _recipeService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Activer / Désactiver une recette
    /// </summary>
    /// <remarks>
    /// Bascule le champ IsActive de la recette. Une recette désactivée n'apparaît plus dans les listes publiques.
    /// </remarks>
    /// <param name="id">Identifiant de la recette</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpPatch("{id:int}/toggle-active")]
    [ProducesResponseType(typeof(RecipeDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailResponse>> ToggleActive(int id, CancellationToken ct)
    {
        var recipe = await _recipeService.ToggleActiveAsync(id, ct);
        return Ok(recipe);
    }

    /// <summary>
    /// Supprimer une recette (soft delete)
    /// </summary>
    /// <remarks>
    /// Marque la recette comme supprimée sans effacer les données.
    /// Requiert l'identifiant de l'utilisateur et une raison obligatoire.
    /// </remarks>
    /// <param name="id">Identifiant de la recette à supprimer</param>
    /// <param name="request">Utilisateur et raison de la suppression</param>
    [Authorize(Roles = AppRoleConstants.ChefOrAbove)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, [FromBody] SoftDeleteRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _recipeService.DeleteAsync(id, request, ct);
        return NoContent();
    }
}
