using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimesNumbersSearchWeb.Controllers;
using PrimesNumbersSearchWeb.Data;
using PrimesNumbersSearchWeb.Hubs;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// MIO
builder.Services.AddSignalR();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession();
// EN MIO

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

//app.UseSession(); // MIO

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// MIO
app.MapHub<MessageHub>("/messagehub");
// END MIO

app.Run();
