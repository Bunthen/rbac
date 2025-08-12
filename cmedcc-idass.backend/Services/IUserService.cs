using System;
using System.Threading.Tasks;

using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Models;

namespace cmedcc_idass.backend.Services;

public interface IUserService
{
    Task<UserUpdateDto>GetUserById(Guid userId);
    Task<User> CreateUser(UserCreateDto userDto);
    Task<User> UpdateUserById(Guid userId, UserUpdateDto userDto);
    Task<User> DeleteUserById(Guid userId);
}