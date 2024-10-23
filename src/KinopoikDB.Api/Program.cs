using KinopoiskDB.Application;
using KinopoiskDB.Dal.PostgreSQL;
using KinopoiskDB.Infrastructure;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var isDev = builder.Environment.IsDevelopment();
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = isDev;
    options.ValidateOnBuild = isDev;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<KinopoiskDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<IKinopoiskService, KinopoiskService>();

builder.Services.AddControllers();
builder.Services.AddRouting(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Request;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
