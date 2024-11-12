using SmsService.Providers;
using SmsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddLogging(configure => configure.AddConsole());

// Set up configuration to toggle between real and mock handlers
var useMockHttpClient = builder.Configuration.GetValue<bool>("UseMockHttpClient"); // Set this in appsettings.json or environment variable

if (useMockHttpClient)
{
    // Use the MockHttpMessageHandler for testing purposes
    builder.Services.AddHttpClient<ISmsProviderClient, ThirdPartySmsProviderClient>()
        .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler(simulateSuccess: true)) // Set to false for failure simulation
        .ConfigureHttpClient(client =>
        {
            client.BaseAddress = new Uri("https://third-party-sms-provider.com");
        });
}
else
{
    // Configure real HttpClient for production
    builder.Services.AddHttpClient<ISmsProviderClient, ThirdPartySmsProviderClient>(client =>
    {
        client.BaseAddress = new Uri("https://third-party-sms-provider.com"); // Replace with actual base URL
    });
}

builder.Services.AddScoped<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<SmsService.Services.SmsService>();

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();