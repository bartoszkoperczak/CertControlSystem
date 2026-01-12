using CertControlSystem.BackgroundServices;
using CertControlSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//konfiguracja bazy danych
var connectionString = builder.Configuration.GetConnectionString("CertDbContext")
    ?? "Server=DESKTOP-3U0S9C4\\SQLEXPRESS;Database=CertDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<CertDbContext>(options =>
    options.UseSqlServer(connectionString));

//obs³uga u¿ytkowników, ról i gotowych widoków logowania
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<CertDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddHostedService<NotificationWorker>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Certificates}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();