using Microsoft.EntityFrameworkCore;
using Resend;
using TravelBook.Api.Data;
using TravelBook.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// ================================================
// SERVER DATABASE
// ================================================

var connectionString =
    builder.Configuration.GetConnectionString("ServerDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'ServerDatabase' was not found.");

builder.Services.AddDbContext<ServerDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// ================================================
// RESEND
// ================================================

var resendApiToken = builder.Configuration["Resend:ApiToken"];

if (string.IsNullOrWhiteSpace(resendApiToken))
{
    throw new InvalidOperationException(
        "Resend API token is not configured.");
}

builder.Services.AddOptions();

builder.Services.AddHttpClient<ResendClient>();

builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = resendApiToken;
});

builder.Services.AddTransient<IResend, ResendClient>();

// ================================================
// TRAVELBOOK SERVICES
// ================================================

builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<ResendEmailService>();

// ================================================
// BUILD APPLICATION
// ================================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();