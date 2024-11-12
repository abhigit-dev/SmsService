using SmsService.Models;
using SmsService.Infrastructure;
using SmsService.Providers;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading.Tasks;

namespace SmsService.Services
{
    public class SmsService
    {
        private readonly IEventBus _eventBus;
        private readonly ISmsProviderClient _smsProviderClient;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IEventBus eventBus, ISmsProviderClient smsProviderClient, ILogger<SmsService> logger)
        {
            _eventBus = eventBus;
            _smsProviderClient = smsProviderClient;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(SendSmsCommand command)
        {
            var retryPolicy = Policy.Handle<Exception>()
                                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                                        onRetry: (exception, timeSpan, retryCount, context) =>
                                        {
                                            _logger.LogWarning("Retry {RetryCount} for SMS to {PhoneNumber} due to error: {ExceptionMessage}", 
                                                retryCount, command.PhoneNumber, exception.Message);
                                        });

            try
            {
                var result = await retryPolicy.ExecuteAsync(() => _smsProviderClient.SendSmsAsync(command.PhoneNumber, command.SmsText));

                if (result)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}", command.PhoneNumber);
                    await _eventBus.PublishAsync(new SmsSentEvent { PhoneNumber = command.PhoneNumber, IsSent = true });
                }
                else
                {
                    _logger.LogWarning("Failed to send SMS to {PhoneNumber}", command.PhoneNumber);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", command.PhoneNumber);
                await _eventBus.PublishAsync(new SmsSentEvent { PhoneNumber = command.PhoneNumber, IsSent = false });
                return false;
            }
        }
    }
}
