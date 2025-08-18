using Microsoft.AspNetCore.Mvc;
using cmedcc_idass.backend.Services;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Exceptions;
using Microsoft.AspNetCore.Authorization;


namespace cmedcc_idass.backend.Controllers;


[ApiController]
[Route("idass/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    //Regiter new user
    [HttpPost("user/register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto NewUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Console.WriteLine("Controller : '" + NewUser.UserName);
            var result = await _adminService.RegisterUser(NewUser);

            return Created(result.UserName, "User created successfully.");
        }
        catch (AppException ex)
        {
            return Conflict("Expection Message From Service :" + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller Exception Message : " + ex.Message);
            return StatusCode(500, "An error occurred while register the user.");
        }
    }

    //Register new rolder
    [HttpPost("role/register")]
    public async Task<IActionResult> RoleRegistering([FromBody] RegisterRoleDto roleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Console.WriteLine("Controller : '" + roleDto.RoleName);
            var result = await _adminService.CreateRole(roleDto);

            return Created(result.Name, "User created successfully.");
        }
        catch (AppException ex)
        {
            return Conflict("Expection Message From Service :" + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller Exception Message : " + ex.Message);
            return StatusCode(500, "An error occurred while register the role.");
        }
    }

    //Assign role to user
    [HttpPost("role/assign")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserDto userRoleAssignDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Console.WriteLine("Controller : '" + userRoleAssignDto.RoleName);
            var result = await _adminService.AssignRoleToUser(userRoleAssignDto);

            return Created(result.message, ":Secceed Message from service");
        }
        catch (AppException ex)
        {
            return Conflict("Expection Message From Service :" + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller Exception Message : " + ex.Message);
            return StatusCode(500, "An error occurred while register the role.");
        }
    }

    //Get list user with role by user id
    [HttpGet("user/getRole")]
    public async Task<IActionResult> GetListUserWithRole([FromBody] string UserName)
    {
        Console.WriteLine(UserName);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Console.WriteLine("Get Role Controller : " + UserName);
            var result = await _adminService.GetListUserWithRole(UserName);
            return Ok(result);
        }
        catch (AppException ex)
        {
            return Conflict("Expection Message From Service :" + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Controller Exception Message : " + ex.Message);
            return StatusCode(500, "An error occurred while register the role.");
        }
    }

    
}