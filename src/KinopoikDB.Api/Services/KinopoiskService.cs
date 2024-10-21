using KinopoiskDB.Application.Dtos;
using System.Text.Json;

namespace KinopoikDB.Api.Services;

public class KinopoiskService : IKinopoiskService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public KinopoiskService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiUrl = configuration["Kinopoisk:ApiUrl"];
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", configuration["Kinopoisk:ApiKey"]);
    }

    public async Task<MoviesDto> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken)
    {
        var hasTitle = !string.IsNullOrEmpty(title);
        var hasYear = year > 1000;
        const string url = "v2.2/films";
        var titlePart = $"?keyword={title}";
        var yearPart = $"&yearFrom={year}&yearTo={year}";

        if (hasTitle && hasYear)
        {
            var requestTitleAndYear = _apiUrl + url + titlePart + yearPart;
            var responseTitleAndYaer = await _httpClient.GetAsync(requestTitleAndYear, cancellationToken).ConfigureAwait(false);
            var jsonResponse = await responseTitleAndYaer.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var moviesDto = JsonSerializer.Deserialize<MoviesDto>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return moviesDto;
        }

        var requestForTitle = _apiUrl + url + titlePart;
        var responseForTitle = await _httpClient.GetStringAsync(requestForTitle, cancellationToken).ConfigureAwait(false);
        //var jsonResponseTitle = await responceForTitle.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var movieDto = JsonSerializer.Deserialize<MoviesDto>(responseForTitle, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return movieDto;

    }

    public async Task<MoviesDto> GetPremieresAsync(int year, string month, CancellationToken cancellationToken)
    {
        const string url = "v2.2/films/premieres";
        var yearAndMonthPart = $"?year={year}&month={month}";

        var request = _apiUrl + url + yearAndMonthPart;
        var response = await _httpClient.GetAsync(request, cancellationToken);
        var contentJson = await response.Content.ReadAsStringAsync();
        var movieDto = JsonSerializer.Deserialize<MoviesDto>(contentJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return movieDto;
    }
}
