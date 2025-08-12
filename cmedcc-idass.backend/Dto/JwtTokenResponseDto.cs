namespace cmedcc_idass.backend.Dto;

public class JwtTokenResponseDto
{
    public string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; }

    // Optional
    public string[]? Roles { get; set; }
}