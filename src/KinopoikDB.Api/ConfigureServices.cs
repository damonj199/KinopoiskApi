using KinopoiskDB.Application;
using KinopoiskDB.Core.Enum;
using KinopoiskDB.Dal.PostgreSQL;
using KinopoiskDB.Dal.PostgreSQL.Repository;
using KinopoiskDB.Infrastructure;
using KinopoiskDB.Infrastructure.Settings;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

namespace KinopoikDB.Api;

public static class ConfigureServices
{
    public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(MapperProfile));
        services.AddScoped<IKinopoiskService, KinopoiskService>();
        services.AddScoped<IMoviesRepository, MoviesRepository>();
        services.AddHttpClient<IKinopoiskService, KinopoiskService>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<KinopoiskSettings>>().Value;
            client.BaseAddress = new Uri(settings.ApiUrl);
            client.DefaultRequestHeaders.Add("X-API-KEY", settings.ApiKey);
        });
        services.AddHostedService<MoviesSyncService>();
        services.AddControllers();
        services.AddRouting(options =>
        {
            options.LowercaseQueryStrings = true;
            options.LowercaseUrls = true;
        });
        services.AddSwaggerGen();
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.Request;
        });
        services.Configure<KinopoiskSettings>(configuration.GetRequiredSection(nameof(KinopoiskSettings)));

        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
        var dataSourceBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var dataSource = dataSourceBuilder.ConnectionString;

        services.AddDbContext<KinopoiskDbContext>(
            options => options
            .UseNpgsql(dataSource)
            .UseCamelCaseNamingConvention());

        NpgsqlConnection.GlobalTypeMapper.MapEnum<Month>();
    }
}
