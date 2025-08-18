using System;
using System.Threading.Tasks;

using cmedcc_idass.backend.Dto;
// using cmedcc_idass.backend.Models;
using Microsoft.AspNetCore.Identity;

namespace cmedcc_idass.backend.Services;

public interface IAuthService
{
    Task<JwtTokenResponseDto> LoginUser(LoginDto loginDto);
    Task<TokenResponseDto> RefreshTokenAsync(string UserId, string refreshToken);
}

