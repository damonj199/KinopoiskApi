using KinopoikDB.Api;

var builder = WebApplication.CreateBuilder(args);

var isDev = builder.Environment.IsDevelopment();
if (isDev)
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = isDev;
    options.ValidateOnBuild = isDev;
});

builder.Services.ConfigureService(builder.Configuration);

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
