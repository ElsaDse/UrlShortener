using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using urlshortener.Models;
using urlshortener.Services;

namespace urlshortener.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IUrlShortenerService _urlService;

    public IndexModel(IUrlShortenerService urlService)
    {
        _urlService = urlService;
    }

    public IList<Url> Urls { get; set; } = new List<Url>();

    public async Task OnGetAsync()
    {
        Urls = await _urlService.GetUrlsByUserAsync(User);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _urlService.DeleteUrlAsync(id, User);
        return RedirectToPage();
    }
}