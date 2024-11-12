namespace SmsService.Models;

public class SmsSentEvent
{

    public required string PhoneNumber { get; init; }
    public bool IsSent { get; init; }
}