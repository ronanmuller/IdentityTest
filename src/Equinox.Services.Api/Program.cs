using Equinox.Domain.Commands;
using Equinox.Infra.CrossCutting.Identity;
using Equinox.Infra.Data.Context;
using Equinox.Services.Api.Configurations;
using MediatR;
using Microsoft.AspNetCore.Identity;
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

// WebAPI Config
builder.Services.AddControllers();

// Setting DBContexts
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// ASP.NET Identity Settings & JWT
builder.Services.AddApiIdentityConfiguration(builder.Configuration);

// Interactive AspNetUser (logged in)
// NetDevPack.Identity dependency
builder.Services.AddAspNetUserConfiguration();

// AutoMapper Settings
builder.Services.AddAutoMapperConfiguration();

// Adding MediatR for Domain Events and Notifications
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CustomerCommand).Assembly));

// Swagger Config
builder.Services.AddSwaggerConfiguration();

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(c =>
{
    c.AllowAnyHeader();
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
});

// NetDevPack.Identity dependency
app.UseAuthConfiguration();

app.MapControllers();

app.UseSwaggerSetup();

app.Run();