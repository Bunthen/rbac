using Microsoft.AspNetCore.Identity;

namespace cmedcc_idass.backend.Models;

public class ApplicationUser : IdentityUser
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? InactivatedAt { get; set; }
    public string? InactivatedBy { get; set; }
}