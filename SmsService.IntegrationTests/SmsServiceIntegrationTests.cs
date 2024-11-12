using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SmsService.IntegrationTests
{
    [TestFixture]
    public class SmsControllerTests
    {
        private HttpClient _client;
        private string _baseUrl = "http://localhost:5064";  

        [SetUp]
        public void SetUp()
        {
            _client = new HttpClient();
        }

        [Test]
        public async Task SendSms_ReturnsSuccess_WhenSmsIsSent()
        {
            // Arrange
            var requestBody = new
            {
                phoneNumber = "12313123",
                smsText = "Test message"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_baseUrl}/api/Sms/send", jsonContent);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode, "Expected HTTP status code 200 OK");

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("SMS sent successfully", responseContent, "Expected success message in response body");
        }

        [Test]
        public async Task SendSms_ReturnsError_WhenSmsFailsToSend()
        {
            // Arrange
            var requestBody = new
            {
                phoneNumber = "12313123",
                smsText = "Test message"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_baseUrl}/api/Sms/send", jsonContent);

            // Assert
            Assert.AreEqual(500, (int)response.StatusCode, "Expected HTTP status code 500 Internal Server Error");

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Failed to send SMS", responseContent, "Expected failure message in response body");
        }
    }
}
