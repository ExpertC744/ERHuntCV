using ERHuntCV.Repositories;
using HuntCV_Portal.Repositories;
using System.Data.Common;
using ERHuntCV.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// MVC
// =========================================================

builder.Services.AddControllersWithViews();

// =========================================================
// EMAIL SERVICE
// =========================================================

builder.Services.AddScoped<EmailService>();

// =========================================================
// DATABASE
// =========================================================

builder.Services.AddScoped<DbConnection>(sp =>
{
    var configuration =
        sp.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration.GetConnectionString(
            "DefaultConnection");

    return new SqlConnection(connectionString);
});

// =========================================================
// SESSION
// =========================================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpClient();

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromMinutes(60);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});

// =========================================================
// REPOSITORIES
// =========================================================

builder.Services.AddScoped<AccountRepository>();

builder.Services.AddScoped<CandidateProfileRepository>();

builder.Services.AddScoped<OrganizationRepository>();

// =========================================================
// BUILD APPLICATION
// =========================================================

var app = builder.Build();

// =========================================================
// ERROR HANDLING
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Session must be before controller routes
app.UseSession();

app.UseAuthorization();

// =========================================================
// DEFAULT ROUTE
// =========================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();