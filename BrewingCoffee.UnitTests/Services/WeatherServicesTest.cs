#region Using Namespaces

using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using Moq;
using Xunit;
using BrewingCoffee.Helpers;
using BrewingCoffee.Models;
using BrewingCoffee.Services;

#endregion

namespace BrewingCoffee.UnitTests.Services
{
    public class WeatherServicesTest
    {
        #region Internal Members

        private WeatherServices _weatherSvc;

        #endregion

        # region Constructors

        public WeatherServicesTest()
        {
            _weatherSvc = new WeatherServices();
        }

        #endregion

        [Theory]
        [InlineData("http://api.openweathermap.org/data/2.5/weatherServices?q=Sydney,au&APPID=randomKey")]
        [InlineData("http://api.openweathermap.org/data/2.5/weatherServices?q=Sydney,au&APPID=moreRandomKey")]
        public async Task GetCurrentTemperature_WithIncorrectUriToken_ReturnsNullValue(string apiUrl)
        {
            // Arrange

            // Act
            double? response = await _weatherSvc.GetCurrentTemperatureAsync(apiUrl);

            // Assert
            Assert.Null(response);

        }

        [Theory]
        [InlineData("http://api.openweathersmap.org/data/2.5/weatherServices?q=Sydney,au&APPID=034bcc05baab4e7f69628a1a45fcf9af")]
        [InlineData("http://api.openweathermaps.org/data/2.5/weatherServices?q=Sydney,au&APPID=034bcc05baab4e7f69628a1a45fcf9af")]
        public async Task GetCurrentTemperature_WithIncorrectDomain_ReturnsNullValue(string apiUrl)
        {
            // Arrange

            // Act
            double? response = await _weatherSvc.GetCurrentTemperatureAsync(apiUrl);

            // Assert
            Assert.Null(response);

        }

        [Theory]
        [InlineData("http://api.openweathermap.org/data/2.5/weather?q=Sydney,au&APPID=034bcc05baab4e7f69628a1a45fcf9af")]
        public async Task GetCurrentTemperature_WithValidUrl_ReturnsValidResponse(string apiUrl)
        {
            // Arrange

            // Act
            double? response = await _weatherSvc.GetCurrentTemperatureAsync(apiUrl);

            // Assert
            Assert.NotNull(response);

        }
    }
}