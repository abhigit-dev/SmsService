using Moq;
using SmsService.Providers;
using SmsService.Infrastructure;
using Microsoft.Extensions.Logging;
using SmsService.Models;
using SmsService.Tests.Helper;

namespace SmsService.Tests
{
    [TestFixture]
    public class SmsServiceTests
    {
        private Mock<ISmsProviderClient> _smsProviderClientMock;
        private Mock<IEventBus> _eventBusMock;
        private Mock<ILogger<SmsService.Services.SmsService>> _loggerMock;
        private SmsService.Services.SmsService _smsService;

        [SetUp]
        public void Setup()
        {
            _smsProviderClientMock = new Mock<ISmsProviderClient>();
            _eventBusMock = new Mock<IEventBus>();
            _loggerMock = new Mock<ILogger<SmsService.Services.SmsService>>();
            _smsService = new SmsService.Services.SmsService( _eventBusMock.Object, _smsProviderClientMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SendSmsAsync_ShouldSendSmsAndPublishEvent_WhenSmsIsSentSuccessfully()
        {
            // Arrange
            var sendSmsCommand = new SendSmsCommand()
            {
                PhoneNumber = "234243",
                SmsText = "Test Message"
            };
            var smsSentEvent = new SmsSentEvent()
            {
                PhoneNumber = "234243", 
                IsSent = true
            };
            
            _smsProviderClientMock.Setup(client => client.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _eventBusMock.Setup(bus => bus.PublishAsync(It.IsAny<SmsSentEvent>())).Returns(Task.CompletedTask);

            // Act
            var result = await _smsService.SendSmsAsync(sendSmsCommand);

            // Assert
            Assert.IsTrue(result);
            _smsProviderClientMock.Verify(client => client.SendSmsAsync("234243", "Test Message"), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<SmsSentEvent>()), Times.Once);
        }

        [Test]
        public async Task SendSmsAsync_ShouldNotPublishEvent_WhenSmsSendingFails()
        {
            // Arrange
            var sendSmsCommand = new SendSmsCommand()
            {
                PhoneNumber = "234243",
                SmsText = "Test Message"
            };
            var smsSentEvent = new SmsSentEvent()
            {
                PhoneNumber = "234243", 
                IsSent = false
            };
            _smsProviderClientMock.Setup(client => client.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _smsService.SendSmsAsync(sendSmsCommand);

            // Assert
            Assert.IsFalse(result);
            _smsProviderClientMock.Verify(client => client.SendSmsAsync("234243", "Test Message"), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishAsync(smsSentEvent), Times.Never);
        }

        [Test]
        public async Task SendSmsAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var sendSmsCommand = new SendSmsCommand()
            {
                PhoneNumber = "234243",
                SmsText = "Test Message"
            };

            _smsProviderClientMock.Setup(client => client.SendSmsAsync(sendSmsCommand.PhoneNumber, sendSmsCommand.SmsText))
                .ThrowsAsync(new System.Exception("Simulated Exception"));

            // Act
            var result = await _smsService.SendSmsAsync(sendSmsCommand);

            // Assert
            Assert.IsFalse(result);
            LoggerHelper.VerifyLogger(_loggerMock, LogLevel.Error, $"Error sending SMS to",times:Times.Once());

        }
    }
}
