namespace OnlineShopping.Api.Messaging;

using System.Text;
using System.Text.Json;
using OnlineShopping.Api.Dtos;
using OnlineShopping.Api.Interfaces;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

public class CartSyncBackgroundWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CartSyncBackgroundWorker> _logger;
    private IEnvironment? _environment;
    private IConnection? _connection;
    private IConsumer? _consumer;

    private const string queueName = "product-updates-queue";

    public CartSyncBackgroundWorker(IServiceProvider serviceProvider, ILogger<CartSyncBackgroundWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CartSyncBackgroundWorker is starting...");

        await ConnectToMessageBroker();

        _consumer = await _connection.ConsumerBuilder()
            .Queue(queueName)
            .MessageHandler(async (context, message) =>
            {
                try
                {
                    var productEvent = ExtractMessageContent(message.Body());

                    if (productEvent != null)
                    {
                        _logger.LogInformation(" [x] Received product update for Product ID: {Id}", productEvent.Id);

                        // 4. Use DI Scope to resolve your Cart service and update the NoSQL DB
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();

                            var result = await cartService.UpdateItemAsync(productEvent.Id, productEvent);

                            if (result.IsSuccess)
                            {
                                context.Accept();
                            }
                            else
                            {
                                _logger.LogWarning("Skipping message: {Error}", result.ErrorMessage);
                                context.Discard();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing product update message.. Retrying...");

                    context.Requeue();
                }
            })
            .BuildAndStartAsync();

        _logger.LogInformation("CartSyncBackgroundWorker is now listening for messages...");

        // Keep the background service alive until application shutdown is requested
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private ProductUpdatedEvent? ExtractMessageContent(byte[] bodyBytes)
    {
        if (bodyBytes == null)
            return null;

        string jsonString = Encoding.UTF8.GetString(bodyBytes);
        return JsonSerializer.Deserialize<ProductUpdatedEvent>(jsonString);
    }

    private async Task ConnectToMessageBroker()
    {
        const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";
        var settings = ConnectionSettingsBuilder.Create()
            .Uri(new Uri(brokerUri))
            .ContainerId("cart-api-consumer")
            .Build();

        // 1. Initialize environment and connection
        _environment = AmqpEnvironment.Create(settings);
        _connection = await _environment.CreateConnectionAsync();

        await _connection.Management()
                .Queue(queueName)
                .Type(QueueType.QUORUM)
                .DeclareAsync();
    }

    // Graceful cleanup when the app stops
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CartSyncBackgroundWorker is stopping...");

        if (_consumer != null) await _consumer.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        if (_environment is IAsyncDisposable asyncEnv)
        {
            await asyncEnv.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
