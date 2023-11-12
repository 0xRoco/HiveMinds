using HiveMinds.API.Services;
using HiveMinds.API.Services.Interfaces;
using HiveMinds.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<DatabaseContext>(options => options.UseMySQL(builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("LocalConnection") ?? "NULL"
    : builder.Configuration.GetConnectionString("DevConnection") ?? "NULL"), ServiceLifetime.Transient);

builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IThoughtRepository, ThoughtRepository>();
builder.Services.AddTransient<IThoughtService, ThoughtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();