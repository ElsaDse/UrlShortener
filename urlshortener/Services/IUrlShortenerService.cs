using System.Security.Claims;
using urlshortener.Models;

namespace urlshortener.Services;

public interface IUrlShortenerService
{
    Task<string> CreateShortUrlAsync(string longUrl, ClaimsPrincipal? user, DateTime? expiresAt = null);
    Task<Url?> GetUrlByShortCodeAsync(string shortCode);
    Task<IList<Url>> GetUrlsByUserAsync(ClaimsPrincipal user);
    Task<bool> DeleteUrlAsync(int id, ClaimsPrincipal user);
}