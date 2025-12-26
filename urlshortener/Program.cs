using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using urlshortener.Data;
using urlshortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le DbContext avec SQLite (idéal pour dev) ou SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Data Source=urlshortener.db"));

// Ajouter Identity
/*builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();*/

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();  // Ajoute les pages Login/Register par défaut


// Add services to the container.
builder.Services.AddRazorPages();

// (Optionnel mais recommandé) Ajouter ton service personnalisé
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Route spéciale pour la redirection des liens courts : /{shortCode}
//app.MapFallbackToPage("/Redirect", "/Redirect");  // On va créer cette page après

app.Run();
