using Hangfire;
using HAS.Infrastructure;
using HAS.Infrastructure.Persistence.Context;
using HAS.Job.Jobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Register Infrastructure services (repositories, email service, etc.)
builder.Services.AddInfrastructureServices();

// Hangfire Configuration
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Hangfire Server (Worker)
builder.Services.AddHangfireServer();

// Register Job
builder.Services.AddScoped<ReminderBackgroundJob>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Schedule Recurring Jobs
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<ReminderBackgroundJob>(
    "send-appointment-reminders",
    job => job.SendRemindersAsync(),
    Cron.Hourly);

app.Run();
