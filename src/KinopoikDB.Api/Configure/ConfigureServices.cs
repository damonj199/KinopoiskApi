﻿using System.Net;
using System.Reflection;

using KinopoiskDB.Application;
using KinopoiskDB.Dal.PostgreSQL;
using KinopoiskDB.Dal.PostgreSQL.Repository;
using KinopoiskDB.Infrastructure;
using KinopoiskDB.Infrastructure.Settings;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Polly;
using Polly.Extensions.Http;

namespace KinopoikDB.Api.Configure;

public static class ConfigureServices
{
    public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IKinopoiskService, KinopoiskService>();
        services.AddScoped<IMoviesRepository, MoviesRepository>();
        services.AddSingleton<ISyncService, SyncService>();

        services.AddAutoMapper(typeof(MapperProfile));
        services.AddHttpClient<IKinopoiskService, KinopoiskService>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<KinopoiskSettings>>().Value;
            client.BaseAddress = new Uri(settings.ApiUrl);
            client.DefaultRequestHeaders.Add("X-API-KEY", settings.ApiKey);
        }).AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddHostedService<MoviesSyncService>();
        services.AddControllers();
        services.AddRouting(options =>
        {
            options.LowercaseQueryStrings = true;
            options.LowercaseUrls = true;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "My Kinopoisk API",
                Description = "Здесь можно будет получать списки фильмов по названию, с фильтрацией по жанрам и странам, и премьеры за любой месяц"
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.Request;
        });
        services.Configure<KinopoiskSettings>(configuration.GetRequiredSection(nameof(KinopoiskSettings)));
        services.AddDbContext<KinopoiskDbContext>(
            options => options
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseCamelCaseNamingConvention());
        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = configuration.GetConnectionString("Redis");
        });
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");

                });
    }

    static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
