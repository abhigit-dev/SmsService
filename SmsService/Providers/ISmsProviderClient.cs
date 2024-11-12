namespace SmsService.Providers
{
    public interface ISmsProviderClient
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
    }
}