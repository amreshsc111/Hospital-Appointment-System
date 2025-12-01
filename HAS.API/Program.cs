using HAS.API.Endpoints;
using HAS.API.Middleware;
using HAS.Application;
using HAS.Infrastructure;
using HAS.Infrastructure.Identity;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using HAS.Domain.Enums;

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
builder.Services.AddAuthorizationPolicies();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });

    // Apply security requirement per-endpoint based on .RequireAuthorization() or .AllowAnonymous()
    options.OperationFilter<HAS.API.Filters.SecurityRequirementsOperationFilter>();
});
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

// Auth endpoints
app.MapAuthLogin();
app.MapRegister();
app.MapRefreshToken();

// Hospital Management endpoints
app.MapDepartmentEndpoints();
app.MapDoctorEndpoints();
app.MapPatientEndpoints();
app.MapAppointmentEndpoints();

// Enhancement endpoints
app.MapDoctorScheduleEndpoints();
app.MapDoctorLeaveEndpoints();
app.MapCancellationPolicyEndpoints();
app.MapAppointmentHistoryEndpoints();

app.MapGet("/", () => Results.Ok("Hospital Appointment System API is running"));

app.Run();
