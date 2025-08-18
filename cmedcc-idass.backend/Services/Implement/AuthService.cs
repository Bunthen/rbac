using Microsoft.AspNetCore.Identity;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Config;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cmedcc_idass.backend.Models;
using cmedcc_idass.backend.Exceptions;

namespace cmedcc_idass.backend.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(
        AuthDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IConfiguration config
        )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _config = config;
    }

    //User Login Service
    public async Task<JwtTokenResponseDto> LoginUser(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return null;
        var checkpass = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!checkpass) return null;
        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiration) = GenerateJwtToken(user, roles);

        var response = new JwtTokenResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiration,
            UserName = user.UserName,
        };
        return response;
    }
    private (string Token, DateTime ExpiresAt) GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            expires: expires,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string UserId,string refreshToken)
    {
        //Find a user
        var user = await _userManager.FindByIdAsync(UserId);

        if (user == null)
            throw new AppException("User not found");
        // 2. Find and validate the refresh token in the database
  
    }

    private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (token, _) = GenerateJwtToken(user, roles);
        return token;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

