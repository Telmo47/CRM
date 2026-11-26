namespace JobNotificationWorker;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;

public class Worker(ILogger<Worker> logger, IConfiguration configuration, IEmailSender emailSender) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. Iniciar o consumidor do RabbitMQ
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

        // 2. Cronjob Diário com PeriodicTimer
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        // Executar uma verificação logo no arranque
        await CheckStaleApplicationsAsync();

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

            // SQL com JOIN para trazer o nome da empresa:
            var sql = """
                SELECT a."Id", a."Titulo", a."DataCriacao", a."Estado", c."Nome" AS "NomeEmpresa"
                FROM "Applications" a
                INNER JOIN "Companies" c ON a."CompanyId" = c."Id"
                WHERE a."Estado" = 0 AND a."DataCriacao" < @DataLimite
            """;

            var vagas = await dbConnection.QueryAsync<StaleApplication>(sql, new { DataLimite = dataLimite });

            logger.LogInformation("⏰ [CRONJOB] Verificação de vagas com mais de 14 dias sem resposta...");

            var totalVagas = 0;
            foreach (var vaga in vagas)
            {
                totalVagas++;
                logger.LogWarning("⚠️ [SEM RESPOSTA HÁ +14 DIAS]: Vaga #{Id} - {Titulo} da empresa {Empresa}", 
                    vaga.Id, vaga.Titulo, vaga.NomeEmpresa);

                var subject = $"Aviso: Vaga sem resposta há mais de 14 dias - {vaga.NomeEmpresa}";
                var htmlBody = $"""
                    <h2>Lembrete de Candidatura Esquecida</h2>
                    <p>Já passaram mais de 14 dias desde que submeteste a tua candidatura para a vaga <strong>{vaga.Titulo}</strong> na empresa <strong>{vaga.NomeEmpresa}</strong>.</p>
                    <p><strong>Data da submissão:</strong> {vaga.DataCriacao:dd/MM/yyyy}</p>
                    <p>Considera enviar um email de follow-up para demonstrar interesse!</p>
                """;

                await Task.Delay(1100); // 1.1 segundos de intervalo

                await emailSender.SendEmailAsync("candidato@jobtracker.com", subject, htmlBody);
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

public record StaleApplication(int Id, string Titulo, DateTime DataCriacao, int Estado, string NomeEmpresa);