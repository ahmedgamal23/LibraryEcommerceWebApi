using LibraryEcommerceWeb.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LibraryEcommerceWeb.Infrastructure.Repositories
{
    public class SmtpClientWrapper : ISmtpClient
    {
        private readonly SmtpClient _smtpClient;
        public SmtpClientWrapper()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true
            };

            _smtpClient.Credentials = new NetworkCredential(
                "am5679456@gmail.com",
                "xnrd lglj raav qwbt"
            );
        }

        public async Task SendMailAsync(MailMessage mailMessage)
        {
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
