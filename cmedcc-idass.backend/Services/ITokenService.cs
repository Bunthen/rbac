using System;
using System.Threading.Tasks;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Models;


namespace cmedcc_idass.backend.Services;


public interface ITokenService
{
    Task<TokenResponseDto> RefreshTokenAsync(string userId, string refreshToken);
    Task<TokenResponse> GenerateTokensOnLoginAsync(ApplicationUser user);
}