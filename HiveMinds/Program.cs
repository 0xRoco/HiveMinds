using HiveMinds.Adapters;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Core;
using HiveMinds.Extensions;
using HiveMinds.Interfaces;
using HiveMinds.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sentry;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = builder.Configuration["Sentry:Dsn"];
        if (builder.Environment.IsDevelopment())
            o.Debug = true;
        o.TracesSampleRate = 0.05;
        o.MinimumEventLevel = LogLevel.Error;
        o.MinimumBreadcrumbLevel = LogLevel.Information;
        o.SendDefaultPii = true;
        o.AttachStacktrace = true;
        o.IsGlobalModeEnabled = true;

        // Filter out health checks so we don't get spammed with transactions
        o.SetBeforeSendTransaction(transaction => transaction.Name.Contains("GET Ping/") ? null : transaction);
    });

    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);

        var noDebugSpam = Environment.GetEnvironmentVariable("NO_DEBUG_SPAM")?.ToLower() == "true";

        if (noDebugSpam) configuration.MinimumLevel.Information();
    });
    
    builder.Services.AddControllersWithViews();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
            options.Events.OnValidatePrincipal = async context =>
            {
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                var apiResponse = await userService.GetUser(context.Principal?.Identity?.Name ?? string.Empty);
                var user = apiResponse?.Data;
                var ipAddress = context.Request.Headers["X-Forwarded-For"].ToString().Split(new[] { ',' })
                    .FirstOrDefault();
                if (apiResponse is { Success: false } || user == null || string.IsNullOrEmpty(user.Username))
                {
                    await SentrySdk.ConfigureScopeAsync(scope =>
                    {
                        scope.User = new User
                        {
                            Username = "Anonymous",
                            IpAddress = ipAddress ?? "Unknown"
                        };
                        return Task.CompletedTask;
                    });
                }
                else
                {
                    await SentrySdk.ConfigureScopeAsync(scope =>
                    {
                        scope.User = new User
                        {
                            Id = user.Id.ToString(),
                            Username = user.Username,
                            IpAddress = ipAddress ?? "Unknown"
                        };

                        return Task.CompletedTask;
                    });
                }
            };
        });
    
    
    builder.Services.AddOptions();
    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.Configure<HiveMindsSettings>(builder.Configuration.GetSection("HiveMindsSettings"));

    builder.Services.AddHttpContextAccessor();

    builder.Services
        .AddHttpClient("HiveMindsAPI",
            client =>
            {
                client.BaseAddress = new Uri(builder.Environment.IsDevelopment()
                    ? builder.Configuration["HiveMindsSettings:ApiUrl:local"] ?? "NULL"
                    : builder.Configuration["HiveMindsSettings:ApiUrl:dev"] ?? "NULL"
                );
                client.Timeout = TimeSpan.FromSeconds(30);
            })
        .AddHttpMessageHandler<BearerTokenHandler>();

    builder.Services.AddSignalR();

    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IThoughtService, ThoughtService>();

    builder.Services.AddTransient<IUtility, Utility>();
    builder.Services.AddTransient<INotificationService, NotificationService>();
    builder.Services.AddTransient<BearerTokenHandler>();

    builder.Services.AddTransient<IModelToViewModelAdapter, ModelToViewModelAdapter>();
    builder.Services.AddLazyResolution();
    
    var app = builder.Build();

    Log.Information("Logging started at {Now}", DateTime.UtcNow.ToString("u"));
    Log.Information("Log directory: {Directory}", "$LogDirectory$");
    Log.Information("Command line arguments: {Args}", $"{string.Join(" ", args)}");

    if (app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseSentryTracing();
    
    app.UseMiddleware<TokenAuthenticationMiddleware>();

    app.UseAuthorization();

    app.MapHub<NotificationHub>("notificationHub");
    
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
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