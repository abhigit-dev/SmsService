# SmsService 

This project is an SMS microservice that interacts with third-party SMS providers to send messages. The microservice is built using ASP.NET Core and implements a clean architecture, utilizing principles like Dependency Injection, Separation of Concerns, and Event-Driven Communication.

## Key Features

- **SMS Sending**: The service communicates with external SMS providers via HTTP clients.
- **In-Memory Event Bus**: An in-memory event bus is used for communication between different components within the service.
- **Application Insights**: Integrated for monitoring and telemetry.
- **Swagger**: API documentation and testing interface.

## Tech Stack

- **ASP.NET Core** for building the microservice.
- **HttpClient** for communicating with external SMS providers.
- **Application Insights** for monitoring.
- **NUnit** for unit and integration tests.
- **Moq** for mocking dependencies in unit tests.
- **Microsoft.AspNetCore.Mvc.Testing** for integration tests.

## Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later)
- An IDE like [Visual Studio](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/) (optional)
- [Postman](https://www.postman.com/) or `curl` for testing HTTP requests

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/abhigit-dev/SmsService.git
   cd SmsService
2. Restore the dependencies:
    ```bash
   dotnet restore
3. Build the project:
   ```bash
   dotnet build
4. Run the application:
   ```bash
   dotnet run
   
**Configuration**
You can configure the use of mock HTTP clients and other settings via the appsettings.json or environment variables.
*  UseMockHttpClient: Set to true to mock HTTP client calls during testing or local development. Set to false for production.

**Endpoints**
* POST /api/Sms/send: Sends an SMS message. Accepts a JSON body with phoneNumber and smsText fields.
Example request:
   ```bash
     curl -X 'POST' \
  'http://localhost:5064/api/Sms/send' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "phoneNumber": "12313123",
  "smsText": "Test message"
  }'

**Testing**
Unit and integration tests are included in the project to verify the functionality of the SMS service.

**Unit Tests**
Unit tests are written using NUnit and are located in the SmsService.Tests project. The tests focus on verifying individual components, such as the SmsService class. The Moq library is used for mocking dependencies like ISmsProviderClient and IEventBus.

To run the unit tests:
`   dotnet test SmsService.Tests`

**Integration Tests**
Integration tests verify the full flow of the service, including HTTP requests to the /api/Sms/send endpoint. They are written using NUnit and the Microsoft.AspNetCore.Mvc.Testing package, which simulates HTTP requests to the service.

To run the integration tests:
`dotnet test SmsService.IntegrationTests`




