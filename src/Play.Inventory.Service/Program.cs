using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add MongoDB repository
builder.Services.AddMongo()
        .AddMongoRepository<InventoryItem>("inventoryitems")
        .AddMongoRepository<CatalogItem>("catalogitems")
        .AddMassTransitWithRabbitMq();

// Register logger
var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});
var logger = loggerFactory.CreateLogger<CatalogClient>();

// Inject CatalogClient with logging
AddCatalogClient(builder, logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

static void AddCatalogClient(WebApplicationBuilder builder, ILogger<CatalogClient> logger)
{
    Random jitterer = new Random();

    // Register HttpClient and Polly policies
    builder.Services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7012");
    })
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                logger.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
            }
        ))
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,
            TimeSpan.FromSeconds(15),
            onBreak: (outcome, timespan) =>
            {
                logger.LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
            },
            onReset: () =>
            {
                logger.LogWarning("Closing the circuit...");
            }
        ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(1)));
}
