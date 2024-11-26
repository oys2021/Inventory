using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailHelper
{
    private readonly IConfiguration _configuration;

    public EmailHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(string recipientEmail, string subject, string body)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            Console.WriteLine($" my dodo{smtpServer}");
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true 
            };

            mailMessage.To.Add(recipientEmail);

            using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                smtpClient.EnableSsl = true; 
                smtpClient.Send(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
