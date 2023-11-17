using Newtonsoft.Json;
using WebApiAutores.Services;

public class WebPurifyService : IWebPurifyService
{
    private readonly string _apiKey;

    public WebPurifyService(IConfiguration configuration)
    {
        _apiKey = configuration["WebPurify:ApiKey"];
    }

    public async Task<string> CheckForProfanity(string text)
    {
        using (var httpClient = new HttpClient())
        {
            var endpoint = "https://api1.webpurify.com/services/rest/";
            var method = "webpurify.live/filter";
            var parameters = $"method={method}&api_key={_apiKey}&text={text}";

            var response = await httpClient.GetAsync($"{endpoint}?{parameters}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
    public string GetApiKey()
    {
        return _apiKey;
    }

}
