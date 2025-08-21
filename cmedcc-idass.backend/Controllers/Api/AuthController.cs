
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
// using cmedcc_idass.backend.Models;

using cmedcc_idass.backend.Exceptions;
using cmedcc_idass.backend.Services;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Config;

namespace cmedcc_idass.backend.Controllers;

[ApiController]
[Route("idass/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Console.WriteLine("Controller : " + loginDto.Email);
            var tokenResponse = await _authService.LoginUser(loginDto);
            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller : '" + ex.Message);
            return Unauthorized("Invalid credentials." + ex.Message);
        }
    }
    [HttpPost("logins")]
    public async Task<IActionResult> LoginUsers([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var tokenResponse = await _authService.LoginUsers(loginDto);
            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller : '" + ex.Message);
            return Unauthorized("Invalid credentials." + ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var tokens = await _authService.RefreshTokenAsync(tokenRequest.UseName, tokenRequest.RefreshToken);
            return Ok(tokens);
        }
        catch (AppException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller : '" + ex.Message);
            return Unauthorized("Invalid credentials." + ex.Message);
        }
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogoutUser()
    {
        return Ok(new { message = "Logout Successfully" });
    }
    
    


}
