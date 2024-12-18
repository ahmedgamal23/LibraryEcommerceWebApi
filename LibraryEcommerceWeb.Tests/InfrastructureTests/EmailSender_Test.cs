using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Infrastructure.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LibraryEcommerceWeb.Tests.InfrastructureTests
{
    [TestFixture]
    public class EmailSender_Test
    {
        #region Test Email Sender Function 5 Test Cases
        [Test]
        public async Task Email_Sender_Test_Valid_Data()
        {
            // Arrange
            var smptClientMock = new Mock<ISmtpClient>();
            smptClientMock
                .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
                .Returns(Task.CompletedTask);

            var emailSender = new EmailSender(smptClientMock.Object);
            var recipient = "ahmed12@gmail.com";
            var subject = "verify email";
            var body = "click here";

            // Act
            await emailSender.SendEmailAsync(recipient, subject, body);

            // Assert
            smptClientMock.Verify(clients => clients.SendMailAsync(
                It.Is<MailMessage>(m =>
                    m.To[0].Address == recipient &&
                    m.Subject == subject &&
                    m.Body == body
                )
            ), Times.Once);
        }

        [Test]
        public void Email_Sender_Test_InValid_Email()
        {
            // Arrange
            var smptClientMock = new Mock<ISmtpClient>();

            var emailSender = new EmailSender(smptClientMock.Object);
            var inValidRecipient = "ahmed";
            var subject = "verify email";
            var body = "click here : Invalid data";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
            {
                await emailSender.SendEmailAsync(inValidRecipient, subject, body);
            });
        }

        [Test]
        public void Email_Sender_Test_Null_Email()
        {
            // Arrange
            var smptClientMock = new Mock<ISmtpClient>();

            var emailSender = new EmailSender(smptClientMock.Object);

            var subject = "verify email";
            var body = "click here : Invalid data";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await emailSender.SendEmailAsync(null!, subject, body);
            });

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await emailSender.SendEmailAsync(string.Empty, subject, body);
            });
        }

        [Test]
        public async Task Email_Sender_Test_Send_Empty_Subject()
        {
            // Arrange
            var smptClientMock = new Mock<ISmtpClient>();
            smptClientMock
                .Setup(clients => clients.SendMailAsync(It.IsAny<MailMessage>()))
                .Returns(Task.CompletedTask);

            var emailSender = new EmailSender(smptClientMock.Object);
            var recipient = "test@gmail.com";
            var subject = string.Empty;
            var body = "this is a test email with empty subject";

            // Act
            await emailSender.SendEmailAsync(recipient, subject, body);

            // Assert
            smptClientMock.Verify(clients => clients.SendMailAsync(
                It.Is<MailMessage>(m =>
                    m.To[0].Address == recipient &&
                    m.Subject == subject &&
                    m.Body == body
                )
            ), Times.Once);
        }

        [Test]
        public void Email_Sender_Test_SmtpClient_Failure()
        {
            // Arrage
            var smtpClientMock = new Mock<ISmtpClient>();

            smtpClientMock
                .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
                .ThrowsAsync(new SmtpException("Failed to send email."));

            var emailSender = new EmailSender(smtpClientMock.Object);
            var recipient = "test@example.com";
            var subject = "Test Subject";
            var body = "Test Body";

            // Act && Assert
            Assert.ThrowsAsync<SmtpException>(async () => {
                await emailSender.SendEmailAsync(recipient, subject, body);
            });
        }
        #endregion
    }
}
