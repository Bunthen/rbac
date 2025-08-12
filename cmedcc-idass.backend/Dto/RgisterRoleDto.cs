using System.ComponentModel.DataAnnotations;

namespace cmedcc_idass.backend.Dto;

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

public class UserRoleUpdateDto
{

    
}
