using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using urlshortener.Models;
using urlshortener.Services;

namespace urlshortener.Pages;

public class RedirectModel : PageModel
{
    private readonly IUrlShortenerService _urlService;

    public RedirectModel(IUrlShortenerService urlService)
    {
        _urlService = urlService;
    }

    public async Task<IActionResult> OnGetAsync(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            return NotFound();

        var url = await _urlService.GetUrlByShortCodeAsync(shortCode);

        // Vérifie si le lien existe et s'il n'est pas expiré
        if (url == null || (url.ExpiresAt.HasValue && url.ExpiresAt < DateTime.UtcNow))
        {
            // Optionnel : tu peux créer une page d'erreur personnalisée
            return RedirectToPage("/Error"); // ou NotFound()
        }

        // Incrémente le compteur de clics
        url.ClickCount++;
        await _urlService.IncrementClickCountAsync(shortCode); // ou directement _context.SaveChanges()

        // Redirection permanente (bon pour le SEO)
        return RedirectPermanent(url.OriginalUrl);
    }
}