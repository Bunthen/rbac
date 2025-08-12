using Microsoft.AspNetCore.Identity;

namespace cmedcc_idass.backend.Models;
public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;
}
