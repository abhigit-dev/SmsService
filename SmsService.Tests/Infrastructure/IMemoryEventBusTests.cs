using Microsoft.Extensions.Logging;
using Moq;
using SmsService.Infrastructure;
using SmsService.Models;


namespace SmsService.Tests
{
    [TestFixture]
    public class InMemoryEventBusTests
    {
        private Mock<ILogger<InMemoryEventBus>> _loggerMock;
        private InMemoryEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<InMemoryEventBus>>();
            _eventBus = new InMemoryEventBus(_loggerMock.Object);
        }

        [Test]
        public async Task PublishAsync_ShouldLogInformation_WhenCalled()
        {
            // Arrange
            var smsSentEvent = new SmsSentEvent
            {
                PhoneNumber = "1234567890",
                IsSent = true
            };

            // Act
            await _eventBus.PublishAsync(smsSentEvent);

            // Assert
            LoggerHelper.VerifyLogger(
                _loggerMock,
                LogLevel.Information,
                $"Event Published: SMS sent status for {smsSentEvent.PhoneNumber} - Sent: {smsSentEvent.IsSent}",
                Times.Once());
        }

        [Test]
        public async Task PublishAsync_ShouldLogCorrectPhoneNumberAndStatus_WhenCalled()
        {
            // Arrange
            var smsSentEvent = new SmsSentEvent
            {
                PhoneNumber = "9876543210",
                IsSent = false
            };

            // Act
            await _eventBus.PublishAsync(smsSentEvent);

            // Assert
            LoggerHelper.VerifyLogger(
                _loggerMock,
                LogLevel.Information,
                $"Event Published: SMS sent status for {smsSentEvent.PhoneNumber} - Sent: {smsSentEvent.IsSent}",
                Times.Once());
        }
    }
}