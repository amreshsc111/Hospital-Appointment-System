using HAS.API.Endpoints;
using HAS.API.Middleware;
using HAS.Application;
using HAS.Infrastructure;
using HAS.Infrastructure.Identity;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add DbContext first (required by repositories)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Register Infrastructure services (repositories, current user service)
builder.Services.AddInfrastructureServices();

// Register Auth Infrastructure services (JWT, Password Hasher, Refresh Token services)
builder.Services.AddAuthInfrastructure(builder.Configuration);

// Register Application services (Mediator, Handlers, Validators, Pipeline Behaviors)
builder.Services.AddApplicationServices();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"];
builder.Services.AddJwtAuthentication(jwtSecret!);

// Add authorization services
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseRouting();

app.UseGlobalExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapAuthLogin();
app.MapRegister();
app.MapRefreshToken();

app.MapGet("/", () => Results.Ok("Hospital Appointment System API is running"));

app.Run();
