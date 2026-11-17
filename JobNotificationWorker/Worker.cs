namespace JobNotificationWorker;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "application_created",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogInformation("🎉 [NOVA CANDIDATURA RECEBIDA]: {Message}", message);
            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: "application_created",
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        logger.LogInformation("🚀 Worker ligado ao RabbitMQ! À escuta da fila 'application_created'...");

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Worker a encerrar...");
        }
    }
}