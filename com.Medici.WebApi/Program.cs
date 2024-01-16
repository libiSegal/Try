using com.Instant.Mishor.Data;
using com.Instant.Mishor.Data.Entities;
using com.Instant.Mishor.Net.Clients;
using com.Instant.Mishor.Net.Clients.Settings;
using com.Medici.WebApi.Auth;
using com.Medici.WebApi.Settings;
using JsonApiDotNetCore.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Http
builder.Services.AddHttpContextAccessor();

// Logger
Serilog.ILogger logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Services.AddSingleton(logger);

// DataContext
Log.Information("Loading data context");

builder.Services.AddDbContext<DataContext>(builder => builder
    .UseSqlServer(@"Data Source=.\SQLEXPRESS;Database=Medici;User Id=sa;Password=12345;TrustServerCertificate=true;")
);

// Auth
Log.Information("Loading authentication");

var authSettings = new AuthSettings();

builder.Configuration.GetSection("Auth").Bind(authSettings);

if (string.IsNullOrEmpty(authSettings.Secret)) throw new MissingFieldException(nameof(authSettings.Secret));

var signingKeyBytes = Encoding.ASCII.GetBytes(authSettings.Secret);

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = "Medici",
        ValidateAudience = true,
        ValidAudience = "Medici",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
    };
});

var jwtUtils = new JwtUtils(authSettings);

builder.Services.AddSingleton(jwtUtils);

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("default", policy => policy.RequireAuthenticatedUser());
});


// JsonApi
builder.Services.AddJsonApi<DataContext>(options =>
{
    options.Namespace = "api";
    options.UseRelativeLinks = true;
    options.IncludeTotalResourceCount = true;
    options.EnableLegacyFilterNotation = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mishor
var mishorSettings = new ApiSettings();

builder.Configuration.GetSection("Mishor").Bind(mishorSettings);

var mishorClient = new ApiClient(mishorSettings);

builder.Services.AddSingleton(mishorClient);

// Build
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Add JsonApiDotNetCore middleware.
app.UseJsonApi();

// Middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
