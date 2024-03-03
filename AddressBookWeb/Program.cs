using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AddressBookWeb.Models.Data;
using Serilog;
var configuration = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
   .CreateLogger();

Log.Logger.Information("Logging is working fine");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Adding entities to the MockDatabase using DataGenerator
for (int i = 0; i < 10; i++) // Adding 10 entities
{
    var entity = DataGenerator.GenerateRandomEntity();
    MockDatabase.CreateEntity(entity);
}

app.Run();