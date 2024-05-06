using System.IdentityModel.Tokens.Jwt;
using System.Text;
using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.API.Repositories;
using HiveMinds.API.Services;
using HiveMinds.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = builder.Configuration["Sentry:Dsn"];
        o.IsGlobalModeEnabled = true;
        
        if (builder.Environment.IsDevelopment())
            o.Debug = true;
        
        o.TracesSampleRate = 0.05;
        o.MinimumEventLevel = LogLevel.Error;
        o.MinimumBreadcrumbLevel = LogLevel.Information;
        o.SendDefaultPii = true;
        o.AttachStacktrace = true;
    });

    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });
    });

    builder.Services.AddMinio(configureClient => configureClient
        .WithEndpoint(builder.Configuration["Minio:Endpoint"])
        .WithCredentials(builder.Configuration["Minio:AccessKey"], builder.Configuration["Minio:SecretKey"])
        .WithSSL());

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:SecretKey"] ??
                    throw new InvalidOperationException("Invalid secret key")))
            };
        });

    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddDbContext<DatabaseContext>(
        options =>
        {
            options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ??
                             throw new InvalidOperationException("Invalid database connection string"));
            options.EnableSensitiveDataLogging();
        });

    builder.Services.AddOptions();

    builder.Services.Configure<HiveMindsConfig>(builder.Configuration.GetSection("HiveMindsSettings"));
    builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("HiveMindsSettings:EmailConfig"));
    
    builder.Services.AddTransient<ISecurityService, SecurityService>();
    builder.Services.AddTransient<IAccountRepository, AccountRepository>();
    builder.Services.AddTransient<IThoughtRepository, ThoughtRepository>();
    builder.Services.AddTransient<IThoughtService, ThoughtService>();
    builder.Services.AddTransient<IAccountFactory, AccountFactory>();
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<IVerificationService, VerificationService>();
    

    var app = builder.Build();

    Log.Information("Logging started at {Now}", DateTime.UtcNow.ToString("u"));
    Log.Information("Log directory: {Directory}", "$LogDirectory$");
    Log.Information("Command line arguments: {Args}", $"{string.Join(" ", args)}");


    app.UseSerilogRequestLogging();

    app.UseSentryTracing();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    

    app.UseHttpsRedirection();

    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}