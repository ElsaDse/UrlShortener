using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using urlshortener.Models;
using urlshortener.Services;

namespace urlshortener.Pages;

[Authorize]
public class EditModel : PageModel
{
    private readonly IUrlShortenerService _urlService;

    public EditModel(IUrlShortenerService urlService)
    {
        _urlService = urlService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Url ShortenedUrl { get; set; } = default!;

    public string ShortUrl => $"{Request.Scheme}://{Request.Host}/{ShortenedUrl.ShortCode}";

    public class InputModel
    {
        public int Id { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var url = await _urlService.GetUrlByIdAsync(id, User); // À implémenter dans le service

        if (url == null)
            return NotFound();

        ShortenedUrl = url;
        Input = new InputModel
        {
            Id = url.Id,
            ExpiresAt = url.ExpiresAt
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _urlService.UpdateExpirationAsync(Input.Id, Input.ExpiresAt, User);

        if (!result)
            return NotFound();

        TempData["SuccessMessage"] = "La date d'expiration a été mise à jour avec succès.";

        return RedirectToPage("./Index");
    }
}