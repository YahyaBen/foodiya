using System.Security.Cryptography;
using System.Text;
using Foodiya.Domain.Configuration;
using Foodiya.Domain.Exceptions;
using Foodiya.Domain.Extensions;
using Foodiya.Application.Interfaces.Auth;
using Foodiya.Domain.Interfaces.Core;
using Foodiya.Domain.Models;
using Foodiya.Domain.Specifications.AppUsers;
using Microsoft.Extensions.Options;

namespace Foodiya.Infrastructure.Security;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IGenericRepository<AppUserRefreshToken> _tokenRepo;
    private readonly IGenericRepository<AppUser> _userRepo;
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenService(
        IGenericRepository<AppUserRefreshToken> tokenRepo,
        IGenericRepository<AppUser> userRepo,
        IOptions<JwtOptions> jwtOptions,
        IDateTimeProvider dateTimeProvider)
    {
        _tokenRepo = tokenRepo;
        _userRepo = userRepo;
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<string> CreateRefreshTokenAsync(int appUserId, CancellationToken ct = default)
    {
        var rawToken = GenerateRawToken();
        var hash = HashToken(rawToken);

        var entity = new AppUserRefreshToken
        {
            AppUserId = appUserId,
            TokenHash = hash,
            ExpiresAtUtc = _dateTimeProvider.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays),
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            Code = EntityCodeGenerator.For("RTK")
        };

        await _tokenRepo.InsertAsync(entity, ct);
        await _tokenRepo.SaveAsync(ct);

        return rawToken;
    }

    public async Task<(AppUser user, string newRefreshToken)> RotateRefreshTokenAsync(
        string rawRefreshToken, int appUserId, CancellationToken ct = default)
    {
        var hash = HashToken(rawRefreshToken);

        var storedToken = _tokenRepo.GetAll()
            .Where(t => t.TokenHash == hash && t.AppUserId == appUserId)
            .FirstOrDefault()
            ?? throw new FoodiyaUnauthorizedException("Invalid refresh token.");

        if (storedToken.RevokedAtUtc is not null)
        {
            // Possible token reuse attack — revoke entire family
            await RevokeAllForUserAsync(appUserId, ct);
            throw new FoodiyaUnauthorizedException("Refresh token has been revoked. All sessions terminated.");
        }

        if (_dateTimeProvider.UtcNow >= storedToken.ExpiresAtUtc)
            throw new FoodiyaUnauthorizedException("Refresh token has expired. Please sign in again.");

        // Revoke the old token
        storedToken.RevokedAtUtc = _dateTimeProvider.UtcNow;

        // Issue replacement
        var newRawToken = GenerateRawToken();
        var newHash = HashToken(newRawToken);

        storedToken.ReplacedByTokenHash = newHash;
        _tokenRepo.Update(storedToken);

        var newEntity = new AppUserRefreshToken
        {
            AppUserId = appUserId,
            TokenHash = newHash,
            ExpiresAtUtc = _dateTimeProvider.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays),
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            Code = EntityCodeGenerator.For("RTK")
        };

        await _tokenRepo.InsertAsync(newEntity, ct);
        await _tokenRepo.SaveAsync(ct);

        var user = await _userRepo.GetSingleAsync(new AppUserForAuthByIdSpecification(appUserId), ct)
            ?? throw new FoodiyaUnauthorizedException("User not found.");

        return (user, newRawToken);
    }

    public async Task RevokeAllForUserAsync(int appUserId, CancellationToken ct = default)
    {
        var activeTokens = _tokenRepo.GetAll()
            .Where(t => t.AppUserId == appUserId && t.RevokedAtUtc == null)
            .ToList();

        foreach (var token in activeTokens)
        {
            token.RevokedAtUtc = _dateTimeProvider.UtcNow;
            _tokenRepo.Update(token);
        }

        await _tokenRepo.SaveAsync(ct);
    }

    // ─── Helpers ────────────────────────────────────────────────

    private static string GenerateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexStringLower(bytes);
    }
}
