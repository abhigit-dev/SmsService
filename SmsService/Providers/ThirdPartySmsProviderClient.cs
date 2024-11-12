using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SmsService.Providers
{
    public class ThirdPartySmsProviderClient : ISmsProviderClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ThirdPartySmsProviderClient> _logger;

        public ThirdPartySmsProviderClient(HttpClient httpClient, ILogger<ThirdPartySmsProviderClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            var requestBody = new { PhoneNumber = phoneNumber, SmsText = message };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                _logger.LogInformation("Sending SMS to {PhoneNumber} via third-party provider", phoneNumber);
                var response = await _httpClient.PostAsync("/api/send-sms", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to send SMS to {PhoneNumber}. Status Code: {StatusCode}", phoneNumber, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending SMS to {PhoneNumber}", phoneNumber);
                return false;
            }
        }
    }
}