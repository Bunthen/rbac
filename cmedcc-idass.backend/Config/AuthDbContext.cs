// using cmedcc_idass.backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace cmedcc_idass.backend.Config;

public class AuthDbContext : IdentityDbContext
{
    // The DbSet property for your User entity.

    // Constructor to configure the DbContext.
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

}