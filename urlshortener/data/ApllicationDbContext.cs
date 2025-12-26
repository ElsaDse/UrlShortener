using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using urlshortener.Models;

namespace urlshortener.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Url> Urls { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuration supplémentaire pour Url
        builder.Entity<Url>(entity =>
        {
            entity.HasIndex(u => u.ShortCode)
                  .IsUnique();

            entity.Property(u => u.OriginalUrl)
                  .IsRequired()
                  .HasMaxLength(2048);

            entity.Property(u => u.ShortCode)
                  .IsRequired()
                  .HasMaxLength(20);

            // Optionnel : index sur UserId pour accélérer les requêtes par utilisateur
            entity.HasIndex(u => u.UserId);
        });
    }
}