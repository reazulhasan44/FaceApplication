using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _config;

        public EmailSender(EmailConfig config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private async Task SendAsync(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_config.SmtpServer, _config.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_config.UserName, _config.Password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    // logging needed
                    Console.Out.WriteLine("" + ex.Message);
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", _config.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color: red;'> {0}</h2>", message.Content) };
            if (message.Attachments != null && message.Attachments.Any())
            {
                int i = 1;
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add("attachment" + i, attachment);
                    i++;
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }
    }
}
