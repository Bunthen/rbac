using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cmedcc_idass.backend.Models;

public class TokenRequest
{
    public string RefreshToken { get; set; }
}

public class RefreshToken
{
    public int Id { get; set; } // Primary key
    public string Token { get; set; } // The actual refresh token string
    public string JwtId { get; set; } // The ID of the JWT it's linked to
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; } // Has the token been used?
    public bool IsRevoked { get; set; } // Has the token been explicitly revoked?

    // Foreign key to link the refresh token to a specific user
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }
}