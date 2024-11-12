namespace SmsService.Models;

public class SendSmsCommand
{
    public required string PhoneNumber { get; init; }
    public required string SmsText { get; init; }
}