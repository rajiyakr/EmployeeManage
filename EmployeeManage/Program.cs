using EmployeeManage.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using EmployeeManage.Hubs;
using System.Threading.Tasks; // Explicitly include this for Task and async/await
using Microsoft.Extensions.Logging; // Include for the ILogger service in the exception handling

using OfficeOpenXml;
using System.ComponentModel;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnection = builder.Configuration.GetConnectionString("dbConnection")
    ?? throw new InvalidOperationException("Connection string 'dbConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(dbConnection));

// Email sender dummy service to avoid exception in RegisterModel
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure SignalR services
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();



var app = builder.Build();

// Seed roles and admin user **before** app.Run()
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "Employee", "Manager" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        string adminEmail = "admin@company.com";
        string adminPassword = "Strong@password123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        // Use the explicit ILogger import
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database with identity data.");
    }
}

// Configure the HTTP request pipeline
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// CRITICAL: SignalR Hub mapping is correct and matches the client's '/chathub'
app.MapHub<ChatHub>("/chathub");

// app.Run must be last
app.Run();

// Dummy email sender
public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"[DummyEmailSender] To: {email}, Subject: {subject}");
        return Task.CompletedTask;
    }
}
