// using cmedcc_idass.backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using cmedcc_idass.backend.Models;


namespace cmedcc_idass.backend.Config;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    // The DbSet property for your User entity.
    //define connection string
   // public DbSet<ApplicationUser> Users { get; set; }


    // Constructor to configure the DbContext.
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Example: change column sizes or configure auditing fields if needed
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.UpdatedBy).HasMaxLength(256);
            entity.Property(e => e.InactivatedAt);
            entity.Property(e => e.InactivatedBy).HasMaxLength(256);
        });
    }
}