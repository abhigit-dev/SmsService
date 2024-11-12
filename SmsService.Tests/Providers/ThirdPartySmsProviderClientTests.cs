using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SmsService.Providers;

namespace SmsService.Tests
{
    [TestFixture]
    public class ThirdPartySmsProviderClientTests
    {
        private Mock<ILogger<ThirdPartySmsProviderClient>> _loggerMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private ThirdPartySmsProviderClient _smsProviderClient;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<ThirdPartySmsProviderClient>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://third-party-sms-provider.com")
            };

            _smsProviderClient = new ThirdPartySmsProviderClient(httpClient, _loggerMock.Object);
        }

        [Test]
        public async Task SendSmsAsync_ShouldReturnTrue_WhenResponseIsSuccessful()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var message = "Test Message";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://third-party-sms-provider.com/api/send-sms")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _smsProviderClient.SendSmsAsync(phoneNumber, message);

            // Assert
            Assert.IsTrue(result);
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Information, $"Sending SMS to {phoneNumber} via third-party provider", Times.Once());
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Information, $"SMS sent successfully to {phoneNumber}", Times.Once());
        }

        [Test]
        public async Task SendSmsAsync_ShouldReturnFalse_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var message = "Test Message";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://third-party-sms-provider.com/api/send-sms")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            var result = await _smsProviderClient.SendSmsAsync(phoneNumber, message);

            // Assert
            Assert.IsFalse(result);
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Information, $"Sending SMS to {phoneNumber} via third-party provider", Times.Once());
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Warning, $"Failed to send SMS to {phoneNumber}. Status Code: {HttpStatusCode.BadRequest}", Times.Once());
        }

        [Test]
        public async Task SendSmsAsync_ShouldLogErrorAndReturnFalse_WhenExceptionIsThrown()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var message = "Test Message";
            var exception = new HttpRequestException("Simulated exception");

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(exception);

            // Act
            var result = await _smsProviderClient.SendSmsAsync(phoneNumber, message);

            // Assert
            Assert.IsFalse(result);
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Information, $"Sending SMS to {phoneNumber} via third-party provider", Times.Once());
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Error, $"Error occurred while sending SMS to {phoneNumber}", exception, Times.Once());
        }

        [Test]
        public async Task SendSmsAsync_ShouldUseCorrectContentType()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var message = "Test Message";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Content.Headers.ContentType.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _smsProviderClient.SendSmsAsync(phoneNumber, message);

            // Assert
            Assert.IsTrue(result);
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Information, $"Sending SMS to {phoneNumber} via third-party provider", Times.Once());
        }
    }
}
