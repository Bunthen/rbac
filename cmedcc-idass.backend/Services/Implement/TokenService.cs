using Microsoft.AspNetCore.Identity;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Config;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cmedcc_idass.backend.Models;
using cmedcc_idass.backend.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace cmedcc_idass.backend.Services;

public class TokenService : ITokenService
{
    private readonly AuthDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public TokenService(
        AuthDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IConfiguration config
        )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _config = config;
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string UserId, string refreshToken)
    {
        //Find a user
        var user = await _userManager.FindByIdAsync(UserId);

        if (user == null)
            throw new AppException("User not found");
        // 2. Find and validate the refresh token in the database

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == UserId);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate <= DateTime.UtcNow)
        {
            if (storedToken != null)
            {
                var userTokens = _dbContext.RefreshTokens.Where(rt => rt.UserId == UserId);
                foreach (var token in userTokens)
                {
                    token.IsRevoked = true;
                }
                await _dbContext.SaveChangesAsync();
            }
            throw new AppException("Invalid or expired refresh token.");
        }
        storedToken.IsRevoked = true;
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = new RefreshTokens
        {
            Token = GenerateRefreshTokenString(),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };
        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken.Token,
            Expiration = newAccessToken.ValidTo
        };

    }
    // This method is also used during the initial login
    public async Task<TokenResponse> GenerateTokensOnLoginAsync(ApplicationUser user)
    {
        var jwtToken = GenerateJwtToken(user);
        var refreshToken = new RefreshTokens
        {
            Token = GenerateRefreshTokenString(),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            IsUsed = false,
            IsRevoked = false,
            JwtId = jwtToken.Id
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken.Token,
            Expiration = jwtToken.ValidTo
        };
    }


    //Tools

    private JwtSecurityToken GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5), // Short-lived access token
            signingCredentials: credentials);

        return token;
    }

    private string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

}