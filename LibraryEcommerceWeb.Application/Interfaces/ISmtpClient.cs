using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LibraryEcommerceWeb.Application.Interfaces
{
    public interface ISmtpClient
    {
        Task SendMailAsync(MailMessage mailMessage);
    }
}
