using Microsoft.AspNetCore.Identity;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Config;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace cmedcc_idass.backend.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(
        AuthDbContext dbContext,
        UserManager<IdentityUser> userManager,
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
        if (!checkpass ) return null;
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

}