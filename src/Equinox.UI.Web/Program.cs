using Equinox.Domain.Commands;
using Equinox.Infra.CrossCutting.Identity;
using Equinox.Infra.Data.Context;
using Equinox.UI.Web.Configurations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Identity;
using NetDevPack.Identity.User;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// ConfigureServices

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddRazorPages();

// Setting DBContexts
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// ASP.NET Identity Settings
builder.Services.AddWebAppIdentityConfiguration(builder.Configuration);

// Authentication & Authorization
builder.Services.AddSocialAuthenticationConfiguration(builder.Configuration);

// Interactive AspNetUser (logged in)
// NetDevPack.Identity dependency
builder.Services.AddAspNetUserConfiguration();

// AutoMapper Settings
builder.Services.AddAutoMapperConfiguration();

// Adding MediatR for Domain Events and Notifications
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CustomerCommand).Assembly));

// .NET Native DI Abstraction
builder.Services.AddDependencyInjectionConfiguration();

var app = builder.Build();

// Configure

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        // Obtém o contexto 
        var context = services.GetRequiredService<EquinoxContext>();
        context.Database.Migrate(); // Aplica as migrations
    }
    catch (Exception ex)
    {
        // Log de erros ou qualquer outra ação que você queira fazer em caso de falha
        Console.WriteLine($"Ocorreu um erro ao migrar o banco de dados: {ex.Message}");
    }
}
else
{
    app.UseExceptionHandler("/error/500");
    app.UseStatusCodePagesWithRedirects("/error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// NetDevPack.Identity dependency
app.UseAuthConfiguration();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
