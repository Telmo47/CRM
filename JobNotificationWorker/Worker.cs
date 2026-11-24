namespace JobNotificationWorker;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Iniciar o consumidor do RabbitMQ
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

        logger.LogInformation("🚀 Worker ligado ao RabbitMQ e à escuta de eventos!");

        //Cronjob Diário com PeriodicTimer
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        // Executar uma verificação logo no arranque para ver na consola
        await CheckStaleApplicationsAsync();

        // E depois repete a cada 24 horas enquanto o serviço correr
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckStaleApplicationsAsync();
        }
    }

    private async Task CheckStaleApplicationsAsync()
    {
        try
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var dbConnection = new NpgsqlConnection(connectionString);

            // Regra dos 14 dias:
            var dataLimite = DateTime.UtcNow.AddDays(-14);

            var sql = """
                SELECT "Id", "Titulo", "DataCriacao", "Estado" 
                FROM "Applications" 
                WHERE "Estado" = 0 AND "DataCriacao" < @DataLimite
            """;

            var vagas = await dbConnection.QueryAsync<StaleApplication>(sql, new { DataLimite = dataLimite });

            logger.LogInformation("⏰ [CRONJOB] Verificação de vagas com mais de 14 dias sem resposta...");

            var totalVagas = 0;
            foreach (var vaga in vagas)
            {
                totalVagas++;
                logger.LogWarning("⚠️ [SEM RESPOSTA HÁ +14 DIAS]: Vaga #{Id} - {Titulo} (Submetida em: {Data})", 
                    vaga.Id, vaga.Titulo, vaga.DataCriacao);
            }

            if (totalVagas == 0)
            {
                logger.LogInformation("✅ Nenhuma vaga pendente há mais de 14 dias encontrada.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar verificação de vagas no PostgreSQL.");
        }
    }
}

public record StaleApplication(int Id, string Titulo, DateTime DataCriacao, int Estado);