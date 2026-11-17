namespace OnlineShopping.Api.Services;

using System;
using System.Text;
using System.Text.Json;
using OnlineShopping.Api.Interfaces;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

public class RabbitMQMessageProducer : IMessageProducer, IAsyncDisposable
{
    private readonly IEnvironment _environment;
    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMQMessageProducer()
    {
        var brokerUri = "amqp://guest:guest@localhost:5672/%2f";
        var settings = ConnectionSettingsBuilder.Create()
            .Uri(new Uri(brokerUri))
            .ContainerId("catalog-api-producer")
            .Build();
     
        _environment = AmqpEnvironment.Create(settings);
    }

    // Helper method to ensure we reuse a single connection safely across requests
    private async Task<IConnection> GetOrCreateConnectionAsync()
    {
        if (_connection != null) return _connection;

        await _connectionLock.WaitAsync();
        try
        {
            if (_connection == null)
            {
                _connection = await _environment.CreateConnectionAsync();

                // Declare the queue once upon connection establishment
                IManagement management = _connection.Management();
                IQueueSpecification queueSpec = management.Queue("product-updates-queue").Type(QueueType.QUORUM);
                await queueSpec.DeclareAsync();
            }
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task PublishProductUpdatedAsync<T>(T messageObject)
    {
        // 2. Get the shared connection
        var connection = await GetOrCreateConnectionAsync();

        // 3. Create the publisher for the queue
        IPublisher publisher = await connection.PublisherBuilder()
            .Queue("product-updates-queue")
            .BuildAsync();

        try
        {
            // Serialize your object into JSON bytes (matching the documentation's Encoding.UTF8.GetBytes)
            var jsonString = JsonSerializer.Serialize(messageObject);
            var message = new AmqpMessage(Encoding.UTF8.GetBytes(jsonString));

            // 4. Publish the message
            PublishResult pr = await publisher.PublishAsync(message);

            if (pr.Outcome.State != OutcomeState.Accepted)
            {
                throw new InvalidOperationException($"Unexpected publish outcome: {pr.Outcome.State}");
            }

            Console.WriteLine($" [x] Product update sent successfully!");
        }
        finally
        {
            // 5. Always close the publisher when done with this specific send operation
            await publisher.CloseAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        // Close the RabbitMQ connection cleanly when the web app shuts down
        if (_connection != null)
        {
            await _connection.CloseAsync();
        }

        // Close the RabbitMQ environment cleanly
        if (_environment is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        _connectionLock.Dispose();
    }
}
