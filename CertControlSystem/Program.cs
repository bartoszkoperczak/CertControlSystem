using CertControlSystem.BackgroundServices;
using CertControlSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Konfiguracja Bazy Danych
var connectionString = builder.Configuration.GetConnectionString("CertDbContext")
    ?? "Server=DESKTOP-3U0S9C4\\SQLEXPRESS;Database=CertDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<CertDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Konfiguracja Identity (Logowanie)
// To dodaje obs³ugê u¿ytkowników, ról i gotowych widoków logowania
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<CertDbContext>();

// 3. Dodanie us³ug MVC (Kontrolery + Widoki)
builder.Services.AddControllersWithViews();

// 4. Dodanie us³ug Razor Pages (WYMAGANE przez Identity UI)
builder.Services.AddRazorPages();

builder.Services.AddHostedService<NotificationWorker>();

var app = builder.Build();

// --- Konfiguracja Potoku (Pipeline) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Domyœlna obs³uga b³êdów
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// najpierw autoryzacja (kto to?), potem uprawnienia (co mo¿e?)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Certificates}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();