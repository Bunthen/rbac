using System.ComponentModel.DataAnnotations;

namespace cmedcc_idass.backend.Dto;

//User Role Defined Object Transfer

public class UserRoleDto
{
    public string UserId { get; set; }
    public string RoleName { get; set; }
    public string UserName { get; set; }
}


public class TokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
public class TokenRequest
{
    public string RefreshToken { get; set; }
}