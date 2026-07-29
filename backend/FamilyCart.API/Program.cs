using FamilyCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FamilyCart.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string" + "'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services.AddAuthorization();

var signingKeyValue = builder.Configuration["Authentication:Schemes:Bearer:SigningKeys:0:Value"] ?? throw new InvalidOperationException("JWT signing key not found.");
var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(signingKeyValue));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Audience = builder.Configuration["Authentication:Schemes:Bearer:ValidAudiences:0"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = options.Audience,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true
    };
});

builder.Services.AddIdentity<User, IdentityRole<int>>(options => 
{
    options.SignIn.RequireConfirmedAccount = false; // keep false for now; testing
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();