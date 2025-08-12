
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using cmedcc_idass.backend.Services;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Config;
using cmedcc_idass.backend.Models;

using cmedcc_idass.backend.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace cmedcc_idass.backend.Controllers;

[ApiController]
[Route("api/auth")]
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


    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutUser()
    {
        return Ok(new { message = "Logout Successfully" });
    }
}
