
using cmedcc_idass.backend.Dto;
using Microsoft.AspNetCore.Identity;
using cmedcc_idass.backend.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
// using cmedcc_idass.backend.Models;

namespace cmedcc_idass.backend.Services;

public class AdminService : IAdminService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminService(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager
    )
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    //Register new user
    public async Task<IdentityUser> RegisterUser(RegisterUserDto newUser)
    {
        var user = new IdentityUser
        {
            UserName = newUser.UserName,
            Email = newUser.Email,
        };

        var result = await _userManager.CreateAsync(user, newUser.Password);
        if (!result.Succeeded)
        {
            throw new AppException(":" + result);
        }
        return user;
    }

    public async Task<IdentityRole> CreateRole(RegisterRoleDto roleDto)
    {
        var role = new IdentityRole
        {
            Name = roleDto.RoleName
        };
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new AppException(":" + result);
        }
        return role;
    }

    public async Task<(bool success, string message)> AssignRoleToUser(AssignRoleToUserDto UserRoleDto)
    {
        var user = await _userManager.FindByIdAsync(UserRoleDto.UserId);
        if (user == null)
        {
            return (false, "User not found");
        }

        if (!await _roleManager.RoleExistsAsync(UserRoleDto.RoleName))
        {
            return (false, "Role not found");
        }
        if (await _userManager.IsInRoleAsync(user, UserRoleDto.RoleName))
        {
            return (false, "User already has this role");
        }
        var result = await _userManager.AddToRoleAsync(user, UserRoleDto.RoleName);
        if (!result.Succeeded)
        {
            return (false, $"Failed to assign role: '{UserRoleDto.RoleName}'");
        }
        return (true, $"Role '{UserRoleDto.RoleName}' assigned to user '{user.UserName}'");
    }


}

