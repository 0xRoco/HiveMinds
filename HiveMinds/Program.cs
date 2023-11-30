using HiveMinds.Adapters;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Common;
using HiveMinds.Extensions;
using HiveMinds.Services;
using HiveMinds.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = builder.Configuration["HiveMindsSettings:SentryDsn"];
        o.Debug = true;
        o.TracesSampleRate = 0.1;
    });

    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));


    builder.Services.AddControllersWithViews();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
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


    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IThoughtService, ThoughtService>();

    builder.Services.AddTransient<IUtility, Utility>();
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