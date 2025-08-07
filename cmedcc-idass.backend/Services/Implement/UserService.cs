using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;
using cmedcc_idass.backend.Config;
using cmedcc_idass.backend.Dto;
using cmedcc_idass.backend.Models;
using cmedcc_idass.backend.Exceptions;


namespace cmedcc_idass.backend.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUser(UserCreateDto userDto)
    {
        var existingUser = await _dbContext.Users
            .Where(u => u.Username == userDto.Username || u.Email == userDto.Email)
            .FirstOrDefaultAsync();

        if (existingUser != null)
        {
            // Throw a custom exception if a user with the same data is found
            Console.WriteLine("User already exists");
            throw new UserExistsException("A user with this username or email already exists.");
        }
        // 1. Create a new User entity and map data from the DTO.
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = userDto.Username,
            Email = userDto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 2. Hash the plain-text password from the DTO before storing.
        user.Salt = GenerateSalt();
        user.PasswordHash = HashPassword(userDto.Password, user.Salt);

        // 3. Add the user to the database context and save changes.
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserById(Guid userId, UserUpdateDto userDto)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return null; // User not found
        }
        // Update properties from the DTO
        if (!string.IsNullOrEmpty(userDto.Username))
        {
            user.Username = userDto.Username;
        }

        if (!string.IsNullOrEmpty(userDto.Email))
        {
            user.Email = userDto.Email;
        }
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return user;

    }

    public async Task<User> DeleteUserById(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return null; // User not found
        }

        user.IsActive = false;
        await _dbContext.SaveChangesAsync();
        return user;

    }


    public async Task<User> GetUserById(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return null; // User not found
        }
        return user;
    }





    // --- Helper methods for password hashing (simplified for demonstration) ---
    public static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }
    private string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        using (var hmac = new HMACSHA256(saltBytes))
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

}