using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking;
using Parcel_Tracking.Controllers;
using Parcel_Tracking.Models;
using Microsoft.Extensions.Options;
using Parcel_Tracking.Hubs;
using Microsoft.AspNetCore.SignalR;
using Parcel_Tracking.Middleware;





var builder = WebApplication.CreateBuilder(args);

// Register the IShipmentService and its implementation in DI container







// Add services to the container.
builder.Services.AddDistributedMemoryCache();  // Use in-memory cache for session data
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();




// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();




builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<PayPalService>();




builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddSession();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});


// Make configuration accessible for DI
builder.Services.Configure<PayPalOptions>(builder.Configuration.GetSection("PayPal"));
builder.Services.AddScoped<PayPalService>();

var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();

app.UseHttpsRedirection();

app.UseStaticFiles(); // This is crucial for serving static files like images

app.UseRouting();

app.MapControllers(); // Ensure controllers are mapped


app.UseAuthentication();
app.UseMiddleware<ForceLogoutMiddleware>();

app.UseAuthorization();



app.MapHub<ChatHub>("/chatHub");



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<LogoutHub>("/logoutHub");
app.MapRazorPages();
app.MapDefaultControllerRoute();

// Seed admin user and role
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create Admin user if it doesn't exist
        var adminEmail = "admin@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@123"); // Set a secure password
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding admin user and role: {ex.Message}");
    }
}



app.Run();
