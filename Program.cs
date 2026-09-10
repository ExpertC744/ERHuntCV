using ERHuntCV.Repositories;
using HuntCV_Portal.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();



// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpClient();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Repository
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<CandidateProfileRepository>();
builder.Services.AddScoped<OrganizationRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();



app.UseStaticFiles();

app.UseRouting();

// IMPORTANT: Session must be before MapControllerRoute
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();