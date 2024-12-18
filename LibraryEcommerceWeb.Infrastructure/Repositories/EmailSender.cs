using LibraryEcommerceWeb.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace LibraryEcommerceWeb.Infrastructure.Repositories
{
    public class EmailSender : IEmailSender
    {
        private ISmtpClient _smtpClient;
        private readonly string _senderEmail;

        public EmailSender(ISmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
            _senderEmail = "am5679456@gmail.com";
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail),
                Body = message,
                Subject = subject,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
