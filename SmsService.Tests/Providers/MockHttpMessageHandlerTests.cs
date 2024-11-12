using System.Net;
using SmsService.Providers;

namespace SmsService.Tests
{
    [TestFixture]
    public class MockHttpMessageHandlerTests
    {
        [Test]
        public async Task SendAsync_ShouldReturnSuccess_WhenSimulateSuccessIsTrue()
        {
            // Arrange
            MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler(simulateSuccess: true);
            var httpClient = new HttpClient(_mockHttpHandler);

            // Act
            var response = await httpClient.GetAsync("https://mockapi.com");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("{\"status\":\"sent\"}", content);
        }

        [Test]
        public async Task SendAsync_ShouldReturnFailure_WhenSimulateSuccessIsFalse()
        {
            // Arrange
            MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler(simulateSuccess: false);
            var httpClient = new HttpClient(_mockHttpHandler);

            // Act
            var response = await httpClient.GetAsync("https://mockapi.com");

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("{\"error\":\"Failed to send SMS\"}", content);
        }
    }
}