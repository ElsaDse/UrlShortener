using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using urlshortener.Models;
using urlshortener.Services;

namespace urlshortener.Pages;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly IUrlShortenerService _urlService;

    public DetailsModel(IUrlShortenerService urlService)
    {
        _urlService = urlService;
    }

    public Url ShortenedUrl { get; set; } = default!;
    public string ShortUrl => $"{Request.Scheme}://{Request.Host}/{ShortenedUrl.ShortCode}";
    public bool IsOwner { get; set; }

    public async Task<IActionResult> OnGetAsync(string shortCode)
    {
        var url = await _urlService.GetUrlByShortCodeAsync(shortCode);
        if (url == null)
            return NotFound();

        ShortenedUrl = url;
        IsOwner = User.Identity?.IsAuthenticated == true && url.UserId == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _urlService.DeleteUrlAsync(id, User);
        return RedirectToPage("/Dashboard/Index");
    }
}