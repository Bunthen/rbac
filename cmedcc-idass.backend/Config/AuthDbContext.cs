// using cmedcc_idass.backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using cmedcc_idass.backend.Models;


namespace cmedcc_idass.backend.Config;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    // The DbSet property for your User entity.
    //define connection string
    public DbSet<ApplicationUser> Users { get; set; }


    // Constructor to configure the DbContext.
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

}