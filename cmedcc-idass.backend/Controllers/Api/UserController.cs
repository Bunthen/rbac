
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using cmedcc_idass.backend.Services;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Config;
using cmedcc_idass.backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using cmedcc_idass.backend.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace cmedcc_idass.backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            Console.WriteLine("Controller : '" + userDto.Username + " " + userDto.Email + " " + userDto.Password);
            var user = await _userService.CreateUser(userDto);

            return Created(user.Username, "User created successfully.");
        }
        catch (UserExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred while creating the user.");
        }
    }
    [Authorize]
    [HttpGet("ById")]
    public async Task<IActionResult> GetUserById([FromBody] UserReadDto userDto)
    {
        Console.WriteLine(userDto.UserId.ToString());
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _userService.GetUserById(userDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred while getting the user.");
        }
    }


    // Soft delete by request body
  
  
        

}
