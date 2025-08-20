using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;
using Task2.Application.Mappings;
using Task2.Core.Interfaces;
using Task2.Infrastructure.Data;
using Task2.Infrastructure.Repositories;
using Task2.Infrastructure.Services;
using Microsoft.Extensions.FileProviders;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

const string CorsPolicy = "AllowFrontend";
builder.Services.AddCors(options =>
{
	options.AddPolicy(CorsPolicy, policy =>
	{
		policy.WithOrigins(
			"http://localhost:3000",
			"https://localhost:3000"
		)
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials();
	});
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task2 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    try
    {
        var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("RedisConnection")!);
        configuration.AbortOnConnectFail = false;
        configuration.ConnectRetry = 3;
        configuration.ReconnectRetryPolicy = new ExponentialRetry(5000);
        
        return ConnectionMultiplexer.Connect(configuration);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Redis connection failed, continuing without cache");
        var fallbackConfig = ConfigurationOptions.Parse("localhost:6379");
        fallbackConfig.AbortOnConnectFail = false;
        return ConnectionMultiplexer.Connect(fallbackConfig);
    }
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JwtSettings are not configured. Please set JwtSettings in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Task2.Application.Commands.RegisterUserCommand).Assembly);
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddValidatorsFromAssembly(typeof(Task2.Application.Validators.RegisterUserCommandValidator).Assembly);

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);

if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

// Ensure images folder exists
var imagesPath = Path.Combine(app.Environment.ContentRootPath, "Image");
if (!Directory.Exists(imagesPath))
{
	Directory.CreateDirectory(imagesPath);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/error");
}

// Serve images by GUID at '/image/{guid}' regardless of file extension
app.MapGet("/image/{guid}", (string guid, IWebHostEnvironment env) =>
{
	var imagesRoot = Path.Combine(env.ContentRootPath, "Image");
	if (!Directory.Exists(imagesRoot))
	{
		return Results.NotFound();
	}
	var matches = Directory.GetFiles(imagesRoot, guid + ".*");
	if (matches.Length == 0)
	{
		return Results.NotFound();
	}
	var filePath = matches[0];
	var provider = new FileExtensionContentTypeProvider();
	if (!provider.TryGetContentType(filePath, out var contentType))
	{
		contentType = "application/octet-stream";
	}
	return Results.File(filePath, contentType);
});

try
{
	Log.Information("Starting Task2 API");
	var address = $"http://{IPAddress.Any.ToString()}:7080";
	app.Urls.Add(address);
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}
