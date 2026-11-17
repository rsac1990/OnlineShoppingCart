namespace OnlineShopping.Api.Messaging;

using System.Text.Json;
using Data.Catalog;
using Dtos;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineShopping.Data.Catalog.Outbox;

public class OutboxProcessorBackgroundWorker : BackgroundService
{
    private const int OutboxBatchSize = 20;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorBackgroundWorker> _logger;

    public OutboxProcessorBackgroundWorker(IServiceProvider serviceProvider,
                                           ILogger<OutboxProcessorBackgroundWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessorBackgroundWorker is starting...");

        // Run an infinite loop that checks for unprocessed outbox messages
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var eventPublisher = scope.ServiceProvider.GetRequiredService<IMessageProducer>();

                    var pendingMessages = dbContext.OutboxMessages
                        .Where(m => m.ProcessedOnUtc == null)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(OutboxBatchSize)
                        .ToList();

                    if (pendingMessages.Count > 0)
                    {
                        _logger.LogInformation("Found {Count} unprocessed outbox message(s). Publishing to RabbitMQ...", pendingMessages.Count);

                        foreach (var message in pendingMessages)
                        {
                            try
                            {
                                await RaiseEvent(eventPublisher, message);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to publish outbox message ID {Id}", message.Id);
                                message.Error = ex.Message;
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing outbox messages.");
            }

            // Wait 5 seconds before checking for new outbox messages again
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("OutboxProcessorBackgroundWorker is stopping...");
    }

    private static async Task RaiseEvent(IMessageProducer eventPublisher, OutboxMessage message)
    {
        if (message.Type == nameof(ProductUpdatedEvent))
        {
            var productEvent = JsonSerializer.Deserialize<ProductUpdatedEvent>(message.Payload);
            if (productEvent != null)
            {
                await eventPublisher.PublishProductUpdatedAsync(productEvent);
            }
        }
        message.ProcessedOnUtc = DateTime.UtcNow;
        message.Error = null;
    }

}
