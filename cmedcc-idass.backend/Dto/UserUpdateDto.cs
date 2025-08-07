using System.ComponentModel.DataAnnotations;


namespace cmedcc_idass.backend.Dto;

public class UserUpdateDto
{
    [MaxLength(256)]
    public string Username { get; set; }

    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; }

    public bool? IsActive { get; set; }
}