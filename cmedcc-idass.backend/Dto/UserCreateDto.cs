using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cmedcc_idass.backend.Dto;

public class UserCreateDto
{
    [Required]
    [MaxLength(256)]
    public string Username { get; set; }

    [Required]
    [EmailAddress] // Added for validation
    [MaxLength(256)]
    [MinLength(4)]
    public string Email { get; set; }

    [Required]
    [MinLength(8)] // Added for validation
    public string Password { get; set; }
}