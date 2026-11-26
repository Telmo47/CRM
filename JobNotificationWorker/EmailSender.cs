namespace JobNotificationWorker;

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}

public class EmailSender(IConfiguration configuration, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var smtpSection = configuration.GetSection("Smtp");
        var host = smtpSection["Host"] ?? "sandbox.smtp.mailtrap.io";
        var port = int.Parse(smtpSection["Port"] ?? "2525");
        var username = smtpSection["Username"];
        var password = smtpSection["Password"];
        var fromEmail = smtpSection["FromEmail"] ?? "notifications@jobtracker.com";

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, "JobTracker Notifier"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        logger.LogInformation("📨 A enviar email para {ToEmail} (Assunto: {Subject})...", toEmail, subject);
        await client.SendMailAsync(mailMessage);
        logger.LogInformation("✅ Email entregue com sucesso no Mailtrap!");
    }
}