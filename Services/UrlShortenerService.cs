using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using urlshortener.Data;
using urlshortener.Models;
using System.Security.Claims;

namespace urlshortener.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private const int CodeLength = 7; // Tu peux ajuster (6-8 est bien)
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public UrlShortenerService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Génère un code court aléatoire
    private string GenerateShortCode()
    {
        var random = new Random();
        var code = new char[CodeLength];
        for (int i = 0; i < CodeLength; i++)
        {
            code[i] = Chars[random.Next(Chars.Length)];
        }
        return new string(code);
    }

    // Vérifie l'unicité et génère jusqu'à en trouver un unique
    private async Task<string> GetUniqueShortCodeAsync()
    {
        string code;
        do
        {
            code = GenerateShortCode();
        } while (await _context.Urls.AnyAsync(u => u.ShortCode == code));
        return code;
    }

    public async Task<string> CreateShortUrlAsync(string longUrl, ClaimsPrincipal? user, DateTime? expiresAt = null)
    {
        // Validation basique de l'URL
        if (!Uri.TryCreate(longUrl, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("L'URL fournie n'est pas valide.");
        }

        var shortCode = await GetUniqueShortCodeAsync();

        var url = new Url
        {
            OriginalUrl = longUrl,
            ShortCode = shortCode,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt?.ToUniversalTime(),
            ClickCount = 0,
            UserId = user?.Identity?.IsAuthenticated == true 
                ? _userManager.GetUserId(user) 
                : null
        };

        _context.Urls.Add(url);
        await _context.SaveChangesAsync();

        return shortCode;

        // Retourne l'URL courte complète
        //return $"https://mon.domaine/{shortCode}";
        //return $"/{shortCode}";
        //return $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{shortCode}";
        // Ou mieux : injecte IHttpContextAccessor pour construire dynamiquement
    }

    public async Task<Url?> GetUrlByShortCodeAsync(string shortCode)
    {
        return await _context.Urls
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    }

    public async Task<IList<Url>> GetUrlsByUserAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        return await _context.Urls
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteUrlAsync(int id, ClaimsPrincipal user)
    {
        var url = await _context.Urls.FindAsync(id);

        if (url == null)
            return false;

        // Vérification : seul le propriétaire ou admin peut supprimer
        var userId = _userManager.GetUserId(user);
        if (url.UserId != userId)
            return false;

        _context.Urls.Remove(url);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task IncrementClickCountAsync(string shortCode)
    {
        var url = await _context.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        if (url != null)
        {
            url.ClickCount++;
            await _context.SaveChangesAsync();
        }
    }

    // Récupère un lien par son ID, uniquement si l'utilisateur en est le propriétaire
    public async Task<Url?> GetUrlByIdAsync(int id, ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var url = await _context.Urls
            .FirstOrDefaultAsync(u => u.Id == id);

        // Sécurité : seul le propriétaire peut accéder à ses liens
        if (url == null || (url.UserId != null && url.UserId != userId))
        {
            return null;
        }

        return url;
    }

    // Met à jour la date d'expiration d'un lien
    public async Task<bool> UpdateExpirationAsync(int id, DateTime? expiresAt, ClaimsPrincipal user)
    {
        var url = await GetUrlByIdAsync(id, user);

        if (url == null)
        {
            return false; // Lien non trouvé ou pas autorisé
        }

        url.ExpiresAt = expiresAt;

        _context.Urls.Update(url);
        await _context.SaveChangesAsync();

        return true;
    }

}