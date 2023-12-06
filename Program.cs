using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseTrack.Data;
using ExpenseTrack.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ExpenseTrackContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ExpenseTrackContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Middleware to redirect to login page if not authenticated
app.Use(async (context, next) =>
{
    // Exclude specific paths from redirection to avoid a loop
    var path = context.Request.Path;
    if (!context.User.Identity.IsAuthenticated &&
        !path.StartsWithSegments("/Identity/Account/Login") &&
        !path.StartsWithSegments("/Identity/Account/Register") &&
        !path.StartsWithSegments("/Identity/Account/Logout"))
    {
        // Redirect to login page if not authenticated
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "UserProfile",
    pattern: "{controller=UserProfile}/{action=Index}");

app.MapControllerRoute(
    name: "Expense",
    pattern: "{controller=Expense}/{action=Index}");

app.MapRazorPages();

app.Run();
