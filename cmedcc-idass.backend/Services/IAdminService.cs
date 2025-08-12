using System;
using System.Threading.Tasks;
using cmedcc_idass.backend.Dto;
// using cmedcc_idass.backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace cmedcc_idass.backend.Services;

public interface IAdminService
{
    Task<IdentityUser> RegisterUser(RegisterUserDto newUser);
    // Task<IdentityUser> UpdateUserById(Guid userId, UserUpdateDto userDto);
    // Task<IdentityUser> DeleteUserById(Guid userId);
    // Task<UserUpdateDto> GetUserById(Guid userId);
    // Task<List<IdentityUser>> GetAllUsers();

    //Role
    Task<IdentityRole> CreateRole(RegisterRoleDto roleDto);
    // Task<Role> UpdateRoleById(Guid roleId, UserUpdateDto roleDto);


    // // Assign Role to user

    Task<(bool success, string message)> AssignRoleToUser(AssignRoleToUserDto UserRoleDto);
    // Get list user with role
    //Task<List<UserReadDto>> GetListUserWithRole(string UserId);
}

