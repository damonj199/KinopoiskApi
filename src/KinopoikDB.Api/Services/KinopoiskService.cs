using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace KinopoikDB.Api.Services;

public class KinopoiskService : IKinopoiskService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public KinopoiskService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Kinopoisk:ApiKey"];
        _apiUrl = configuration["Kinopoisk:ApiUrl"];
        _httpClient.BaseAddress = new Uri(_apiUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
    }

    public async Task<dynamic> SearchMoviesAsync(string title, int? year, string genre)
    {
        var queryParams = new List<string>();

        var queryString = string.Join("&", queryParams);
        var response = await _httpClient.GetAsync($"/v2.2/films?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(content);
    }

    public async Task<dynamic> GetPremieresAsync(DateTime month)
    {
        var startDate = new DateTime(month.Year, month.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var response = await _httpClient.GetAsync($"/v2.2/films/premieres?start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(content);
    }
}
