using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using urlshortener.Models;
using urlshortener.Services;

namespace urlshortener.Pages;

public class IndexModel : PageModel
{
    private readonly IUrlShortenerService _urlService;

    public IndexModel(IUrlShortenerService urlService)
    {
        _urlService = urlService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ShortUrl { get; set; }

    public class InputModel
    {
        public string LongUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }

    public void OnGet()
    {
        // Rien à faire
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var shortUrl = await _urlService.CreateShortUrlAsync(
            Input.LongUrl,
            User.Identity?.IsAuthenticated == true ? User : null,
            Input.ExpiresAt);

        ShortUrl = shortUrl;
        return Page();
    }
}