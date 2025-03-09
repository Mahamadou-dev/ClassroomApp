using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics; // Pour la gestion des erreurs globales
using System.Text.Json; // Pour la sérialisation des erreurs
var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information)); // Log des requêtes SQL

// Add Controllers
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
});

// Configure CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Autorise toutes les origines
              .AllowAnyMethod() // Autorise toutes les méthodes (GET, POST, etc.)
              .AllowAnyHeader(); // Autorise tous les en-têtes
    });
});

var app = builder.Build();

// Configure Middleware Pipeline

// Configure Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1");
    });
}

// Configure HTTPS redirection
app.UseHttpsRedirection();

// Configure CORS
app.UseCors("AllowAll");

// Configure Authentication and Authorization
app.UseAuthentication(); // Active l'authentification
app.UseAuthorization();

// Global Error Handling Middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            var errorMessage = new
            {
                Message = "An unexpected error occurred.",
                Details = exceptionHandlerPathFeature.Error.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorMessage));
        }
    });
});

// Map Controllers
app.MapControllers();

// Run the application
app.Run();