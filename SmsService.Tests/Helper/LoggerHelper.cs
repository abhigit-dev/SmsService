using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace SmsService.Tests
{
    public static class LoggerHelper
    {
        public static void VerifyLogger<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message, Times times)
        {
            loggerMock.Verify(
                log => log.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                times);
        }

        public static void VerifyLogger<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message, Exception exception, Times times)
        {
            loggerMock.Verify(
                log => log.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    exception,
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                times);
        }
    }
}