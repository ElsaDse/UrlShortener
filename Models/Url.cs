using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace urlshortener.Models;

public class Url
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(2048)]
    public string OriginalUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    //[Index(IsUnique = true)] // Assure l'unicité du code court
    public string ShortCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public int ClickCount { get; set; } = 0;

    // Clé étrangère vers l'utilisateur (optionnel si pas d'auth : nullable)
    public string? UserId { get; set; }

    // Navigation property (facultative mais utile)
    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }
}