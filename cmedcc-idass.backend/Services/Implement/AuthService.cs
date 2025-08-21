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
        var (token, expiration) = GenerateJwtTokens(user, roles);

        var response = new JwtTokenResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiration,
            UserName = user.UserName,
        };
        return response;
    }
    private (string Token, DateTime ExpiresAt) GenerateJwtTokens(IdentityUser user, IList<string> roles)
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

    public async Task<TokenResponseDto> RefreshTokenAsync(string UserId, string refreshToken)
    {
        //Find a user
        var user = await _userManager.FindByNameAsync(UserId);
        Console.WriteLine("Token founded :" + UserId);
        if (user == null)
            throw new AppException("User not found");
        // 2. Find and validate the refresh token in the database
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == user.Id);
        Console.WriteLine("Stored founded :" + storedToken.Token);
        if (storedToken.IsRevoked)
        {
            Console.WriteLine("Expired token");
            throw new AppException("Invalid or expired refresh token.");
        }
        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate <= DateTime.UtcNow)
        {
            if (storedToken != null)
            {
                var userTokens = _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id);
                foreach (var token in userTokens)
                {
                    Console.WriteLine("Token founded :" + token.Token);
                    token.IsRevoked = true;
                }
                Console.WriteLine("Cheche be fore save");
                await _dbContext.SaveChangesAsync();
            }
            throw new AppException("Invalid or expired refresh token.");
        }
        storedToken.IsRevoked = true;
        //_dbContext.RefreshTokens.Update(storedToken);
        //await _dbContext.SaveChangesAsync();
        var newAccessToken = GenerateJwtToken(user);
        Console.WriteLine("1. Token Id:" + newAccessToken.Id);
        var newRefreshToken = new RefreshTokens
        {
            Token = GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            IsUsed = true,
            IsRevoked = true,
            JwtId = newAccessToken.Id
        };
        Console.WriteLine("2. Token Id:" + newRefreshToken.Token);
        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken.Token,
            Expiration = newAccessToken.ValidTo
        };

    }

    // private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    // {
    //     var roles = await _userManager.GetRolesAsync(user);
    //     var (token, _) = GenerateJwtTokens(user, roles);
    //     return token;
    // }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }


    //Generate JWT
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

    //New user login service
    public async Task<TokenResponseDto> LoginUsers(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            Console.WriteLine("Servic : User founded :" + user.UserName);
            var accessToken = await GenerateTokensOnLoginAsync(user);
            Console.WriteLine("Access Token : " + accessToken.ToString());
            var response = new TokenResponseDto
            {
                AccessToken = accessToken.AccessToken,
                RefreshToken = accessToken.RefreshToken,
                Expiration = accessToken.Expiration
            };
            return response;
        }
        else
        {
            return null;

        }

    }

    // This method is also used during the initial login
    public async Task<TokenResponse> GenerateTokensOnLoginAsync(ApplicationUser user)
    {
        var jwtToken = GenerateJwtToken(user);
        Console.WriteLine("Token Id:" + jwtToken.Id);
        var refreshToken = new RefreshTokens
        {
            Token = GenerateRefreshToken(),
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
    
    
}

