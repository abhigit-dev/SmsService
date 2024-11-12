using SmsService.Models;

namespace SmsService.Infrastructure
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly ILogger<InMemoryEventBus> _logger;

        public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(SmsSentEvent smsSentEvent)
        {
            _logger.LogInformation("Event Published: SMS sent status for {PhoneNumber} - Sent: {IsSent}",
                smsSentEvent.PhoneNumber, smsSentEvent.IsSent);
            
            // Here, we can publish the event to the actual event bus (Azure Event hub)
            return Task.CompletedTask;
        }
    }
}