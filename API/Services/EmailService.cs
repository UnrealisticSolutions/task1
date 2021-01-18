using System;
using MailKit.Net.Smtp;
using MailKit;
using System.Threading.Tasks;
using API.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace API.Services
{
    public interface IMailer
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
    public class EmailService:IMailer
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IWebHostEnvironment _env;

        

        public EmailService(IOptions<SmtpSettings> smtpSettings,IWebHostEnvironment env)
        {
            _smtpSettings = smtpSettings.Value;
            _env = env;
        }

        public async Task SendEmailAsync(string email,string subject,string body)
        {
            var senderName = "API";
            var senderEmail = "";
            var server = "smtp.gmail.com";
            var port = 465;
            var password = "";

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(subject,email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };

                using(var client = new SmtpClient())
                {
                    client.Connect(server, port, true);
                    client.Authenticate(senderEmail, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
