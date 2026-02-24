using LibPro.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LibproContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibProContext")));

builder.Services.AddAuthentication("ManagerLogin").AddCookie("ManagerLogin", options =>
{
    options.LoginPath = "/Login/Login";
    options.LogoutPath = "/Login/Logout";
    options.AccessDeniedPath = "/Home/Index";
});


var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    SeedData.Initialize(scope.ServiceProvider);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
