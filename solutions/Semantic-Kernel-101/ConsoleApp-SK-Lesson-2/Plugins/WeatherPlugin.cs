using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http.Json;

namespace SKTraining.Plugins
{
    public class WeatherResponse
    {
        public MainInfo? Weather { get; set; }
    }

    public class MainInfo
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
    }

    public class WeatherPlugin
    {
        string WeatherPluginEndpoint = ConfigurationManager.AppSettings.Get("WeatherEndpoint") ?? ""; 
        string WeatherAPIKey = ConfigurationManager.AppSettings.Get("WeatherAPIKey") ?? ""; 
        const string units = "imperial";

        private readonly HttpClient _client = new();

        [KernelFunction]
        [Description("Retreives the current weather for any zip code in the US")]
        public async Task<string> GetWeatherAsync([Description("zip code for the weather")] string zipcode)
        {
            var openWeatherEndpoint = $@"{WeatherPluginEndpoint}{zipcode},us&appid={WeatherAPIKey}&units={units}";
            HttpRequestMessage request = new(HttpMethod.Get, openWeatherEndpoint);

            var response = await _client.SendAsync(request).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return result;
        }
    }
}
