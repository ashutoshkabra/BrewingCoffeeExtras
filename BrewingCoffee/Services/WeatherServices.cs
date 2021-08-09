#region Using Namespaces

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Newtonsoft.Json;
using BrewingCoffee.Models;

#endregion

namespace BrewingCoffee.Services
{
    public interface IWeatherServices
    {
        Task<double?> GetCurrentTemperatureAsync(string apiUrl);
    }

    public class WeatherServices : IWeatherServices
    {
        #region Internal Members

        private static readonly HttpClient client = new HttpClient();
        private const string _apiUrl = "http://api.openweathermap.org/data/2.5/weatherServices?q=Sydney,au&APPID=034bcc05baab4e7f69628a1a45fcf9af";

        #endregion

        #region Public Methods

        public async Task<double?> GetCurrentTemperatureAsync(string apiUrl)
        {
            // Set defaults if blank
            apiUrl = string.IsNullOrWhiteSpace(apiUrl) ? _apiUrl : apiUrl;

            double? resultOut = null;

            try
            {
                string weatherResponse = await CallWeatherApiAsync(apiUrl);
                Root jsonObj = JsonConvert.DeserializeObject<Root>(weatherResponse);
                // Convert temperature from Kelvin to Celsius
                resultOut = jsonObj.main.temp - 273.15;
            }
            catch (HttpRequestException)
            {
                resultOut = null;
            }
            catch (JsonSerializationException)
            {
                resultOut = null;
            }

            return resultOut;
        }

        #endregion

        #region Internal Methods

        private async Task<string> CallWeatherApiAsync(string apiUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET WeatherServices");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var stringTask = client.GetStringAsync(apiUrl);

            return await stringTask;
        }

        #endregion
    }
}