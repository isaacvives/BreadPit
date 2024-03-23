using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BreadPit.Data;
using BreadPit.Areas.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BreadPitContextConnection") ?? throw new InvalidOperationException("Connection string 'BreadPitContextConnection' not found.");

builder.Services.AddDbContext<BreadPitContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddDefaultIdentity<BreadPitUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BreadPitContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<BreadPitUser>>();
    bool x = await _roleManager.RoleExistsAsync("Admin");
    if (!x)
    {
        var role = new IdentityRole();
        role.Name = "Admin";
        await _roleManager.CreateAsync(role);
    }

    x = await _roleManager.RoleExistsAsync("Manager");
    if (!x)
    {
        var role = new IdentityRole();
        role.Name = "Manager";
        await _roleManager.CreateAsync(role);
    }

    x = await _roleManager.RoleExistsAsync("User");
    if (!x)
    {
        var role = new IdentityRole();
        role.Name = "User";
        await _roleManager.CreateAsync(role);
    }
    x = await _roleManager.RoleExistsAsync("PendingUser");
    if (!x)
    {
        var role = new IdentityRole();
        role.Name = "PendingUser";
        await _roleManager.CreateAsync(role);
    }
}

app.Run();
