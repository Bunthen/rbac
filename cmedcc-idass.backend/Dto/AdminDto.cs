using System.ComponentModel.DataAnnotations;

namespace cmedcc_idass.backend.Dto;
public class RegisterUserDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}


//User Role Defined Object Transfer
public class UserRoleDto
{
    public string UserId { get; set; }
    public string RoleName { get; set; }
    public string UserName { get; set; }
}

//Update User Defined Object Transfer
public class UserUpdateDto
{
    [MaxLength(256)]
    public string Username { get; set; }

    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; }

    public bool? IsActive { get; set; }
}

public class RegisterRoleDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string RoleName { get; set; }

}

public class AssignRoleToUserDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string RoleName { get; set; }
}
