using Aplicacion.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Aplicacion.Core;
public class EmailSender : IEmailSender
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly string _username;
    private readonly string _password;

    public EmailSender(IConfiguration configuration)
    {
        _smtpServer = configuration["EmailSettings:SmtpServer"]!;
        _smtpPort = int.Parse(configuration["EmailSettings:Port"]!);
        _senderEmail = configuration["EmailSettings:SenderEmail"]!;
        _senderName = configuration["EmailSettings:SenderName"]!;
        _username = configuration["EmailSettings:Username"]!;
        _password = configuration["EmailSettings:Password"]!;
    }

    public async Task<bool> SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            Console.WriteLine($"Email enviado correctamente a {email}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el email: {ex.Message}");
            return false;
        }
    }
}
