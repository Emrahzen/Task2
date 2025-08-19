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
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);

if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler("/error");

try
{
	Log.Information("Starting Task2 API");
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
