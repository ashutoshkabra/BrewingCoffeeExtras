﻿#region Using Namespaces

using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using Moq;
using Xunit;
using BrewingCoffee.Helpers;
using BrewingCoffee.Services;
using BrewingCoffee.Controllers;

#endregion

namespace BrewingCoffee.UnitTests.Controllers
{
    public class CoffeeControllerTest
    {
        #region Internal Members

        private MemoryCache _memoryCache;
        private Mock<ISysTime> _mockISysTime;
        private Mock<IWeatherServices> _mockIWeatherServices;

        private const string _messageHot            = "Your piping hot coffee is ready";
        private const string _messageCold           = "Your refreshing iced coffee is ready";
        private const string _dateFormat_ISO8601    = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";

        #endregion

        #region Constructors

        public CoffeeControllerTest()
        {
            _mockISysTime           = new Mock<ISysTime>();
            _mockIWeatherServices   = new Mock<IWeatherServices>();
            _memoryCache            = new MemoryCache(new MemoryCacheOptions());
        }

        #endregion

        #region Tests

        [Theory]
        [InlineData(200, _messageCold, 29.99)]
        [InlineData(200, _messageCold, 15.45)]
        [InlineData(200, _messageCold, 5)]
        public async Task BrewCoffeeAsync_RequestIsNotFifthCall_ReturnStatusCode200WithColdMessage(int expectedStatusCode, string expectedResult, double currentTemp)
        {
            // Arrange
            DateTime _now = new DateTime(2021, 8, 9);
            _mockISysTime.Setup(gc => gc.GetCurrentTime()).Returns(_now.ToUniversalTime());
            _mockIWeatherServices.Setup(api => api.GetCurrentTemperatureAsync(string.Empty)).ReturnsAsync(currentTemp);
            CoffeeController brewCoffee = new CoffeeController(_memoryCache, _mockISysTime.Object, _mockIWeatherServices.Object);

            // Act
            IActionResult actionResult = await brewCoffee.BrewCoffeeAsync();
            object message = ((ObjectResult)actionResult).Value?.GetType().GetProperty("message")?.GetValue(((ObjectResult)actionResult).Value, null);
            object prepared = ((ObjectResult)actionResult).Value?.GetType().GetProperty("prepared")?.GetValue(((ObjectResult)actionResult).Value, null);

            // Assert
            Assert.Equal(expectedStatusCode, (int) ((ObjectResult) actionResult).StatusCode);
            Assert.Equal(expectedResult, message.ToString());
            Assert.Equal(_now.ToUniversalTime().ToString(_dateFormat_ISO8601), prepared.ToString());
        }

        [Theory]
        [InlineData(200, _messageHot, 30.01)]
        [InlineData(200, _messageHot, 35.98)]
        [InlineData(200, _messageHot, 40.12)]
        public async Task BrewCoffeeAsync_RequestIsNotFifthCall_ReturnStatusCode200WithHotMessage(int expectedStatusCode, string expectedResult, double currentTemp)
        {
            // Arrange
            DateTime _now = new DateTime(2021, 8, 9);
            _mockISysTime.Setup(gc => gc.GetCurrentTime()).Returns(_now.ToUniversalTime());
            _mockIWeatherServices.Setup(api => api.GetCurrentTemperatureAsync(string.Empty)).ReturnsAsync(currentTemp);
            CoffeeController brewCoffee = new CoffeeController(_memoryCache, _mockISysTime.Object, _mockIWeatherServices.Object);

            // Act
            IActionResult actionResult = await brewCoffee.BrewCoffeeAsync();
            object message = ((ObjectResult)actionResult).Value?.GetType().GetProperty("message")?.GetValue(((ObjectResult)actionResult).Value, null);
            object prepared = ((ObjectResult)actionResult).Value?.GetType().GetProperty("prepared")?.GetValue(((ObjectResult)actionResult).Value, null);

            // Assert
            Assert.Equal(expectedStatusCode, (int)((ObjectResult)actionResult).StatusCode);
            Assert.Equal(expectedResult, message.ToString());
            Assert.Equal(_now.ToUniversalTime().ToString(_dateFormat_ISO8601), prepared.ToString());
        }

        [Theory]
        [InlineData(503, 5)]
        public async Task BrewCoffeeAsync_RequestIsFifthCall_ReturnStatusCode500(int expectedStatusCode, int numOfRequests)
        {
            // Arrange
            DateTime _now = new DateTime(2021, 8, 9);
            _mockISysTime.Setup(gc => gc.GetCurrentTime()).Returns(_now.ToUniversalTime());
            _mockIWeatherServices.Setup(api => api.GetCurrentTemperatureAsync(string.Empty)).ReturnsAsync(It.IsAny<double>());
            CoffeeController brewCoffee = new CoffeeController(_memoryCache, _mockISysTime.Object, _mockIWeatherServices.Object);

            // Act
            IActionResult actionResult = await brewCoffee.BrewCoffeeAsync();

            // Simulate request call 5 times
            for (int idx = 0; idx < numOfRequests - 1; idx++)
            {
                actionResult = await brewCoffee.BrewCoffeeAsync();
            }

            // Assert
            Assert.Equal(expectedStatusCode, (int)((ObjectResult)actionResult).StatusCode);
        }

        [Theory]
        [InlineData(418)]
        public async Task BrewCoffeeAsync_SysDateIsAprilFirst_ReturnStatusCode418(int expectedStatusCode)
        {
            // Arrange
            DateTime _now   = new DateTime(2021, 4, 2);
            _mockISysTime.Setup(gc => gc.GetCurrentTime()).Returns(_now.ToUniversalTime());
            _mockIWeatherServices.Setup(api => api.GetCurrentTemperatureAsync(string.Empty)).ReturnsAsync(It.IsAny<double>());
            CoffeeController brewCoffee = new CoffeeController(_memoryCache, _mockISysTime.Object, _mockIWeatherServices.Object);

            // Act
            IActionResult actionResult = await brewCoffee.BrewCoffeeAsync();

            // Assert
            Assert.Equal(expectedStatusCode, (int)((ObjectResult)actionResult).StatusCode);
        }
        
        #endregion
    }
}