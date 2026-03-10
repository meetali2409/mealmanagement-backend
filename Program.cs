using Microsoft.EntityFrameworkCore;
using MealManagement.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MealManagerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o =>
        {
            o.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null
            );
        }
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("https://mealmanagement-frontend.vercel.app")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();