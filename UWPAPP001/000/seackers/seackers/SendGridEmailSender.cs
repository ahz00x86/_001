using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace seackers.Services
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            // Configura el cliente SMTP
            using (var client = new SmtpClient())
            {
                client.Host = _smtpSettings.Host;
                client.Port = _smtpSettings.Port;
                client.EnableSsl = _smtpSettings.UseSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);

                // Crea el mensaje de correo electrónico
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_smtpSettings.UserName);
                    message.To.Add(email);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    // Envía el correo electrónico
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
