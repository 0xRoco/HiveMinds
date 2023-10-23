using HiveMinds.Adapters;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Common;
using HiveMinds.Common.AutoMapper;
using HiveMinds.Database;
using HiveMinds.Extensions;
using HiveMinds.Services;
using HiveMinds.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddDbContext<DatabaseContext>(options => options.UseMySQL(builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("LocalConnection") ?? "NULL"
    : builder.Configuration.GetConnectionString("DevConnection") ?? "NULL"), ServiceLifetime.Transient);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IVerificationService, VerificationService>();
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISecurityService, SecurityService>();
builder.Services.AddTransient<IThoughtRepository, ThoughtRepository>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IThoughtService, ThoughtService>();

builder.Services.AddTransient<IUtility, Utility>();

builder.Services.AddTransient<IAccountFactory, AccountFactory>();
builder.Services.AddTransient<IModelToViewModelAdapter, ModelToViewModelAdapter>();
builder.Services.AddLazyResolution();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();