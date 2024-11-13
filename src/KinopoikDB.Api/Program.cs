using KinopoikDB.Api.Configure;

using KinopoiskDB.Dal.PostgreSQL;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var isDev = builder.Environment.IsDevelopment();
if (isDev)
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Configuration.AddEnvironmentVariables();
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = isDev;
    options.ValidateOnBuild = isDev;
});

builder.Services.ConfigureService(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Kinopoisk API V1");
    });
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KinopoiskDbContext>();
    dbContext.Database.Migrate();
}
app.UseHttpsRedirection();

app.UseRouting();

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
