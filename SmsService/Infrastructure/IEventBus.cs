using SmsService.Models;

namespace SmsService.Infrastructure
{
    public interface IEventBus
    {
        Task PublishAsync(SmsSentEvent smsSentEvent);
    }
}