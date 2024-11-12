using System.Net;

namespace SmsService.Providers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly bool _simulateSuccess;

        // Constructor takes a flag to indicate success or failure simulation
        public MockHttpMessageHandler(bool simulateSuccess)
        {
            _simulateSuccess = simulateSuccess;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage mockResponse;

            if (_simulateSuccess)
            {
                // Simulate a successful response
                mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"status\":\"sent\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            else
            {
                // Simulate a failure response
                mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("{\"error\":\"Failed to send SMS\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }

            return Task.FromResult(mockResponse);
        }
    }
}