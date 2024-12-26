using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using FurnitureShop.Models;

namespace FurnitureShop.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings; // Sử dụng EmailSettings thay vì SmtpSettings

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Tạo MimeMessage từ MimeKit
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            mimeMessage.To.Add(MailboxAddress.Parse(email)); // Người nhận
            mimeMessage.Subject = subject;

            var body = new TextPart("html") // Định dạng HTML
            {
                Text = message
            };

            mimeMessage.Body = body;

            // Tạo đối tượng SmtpClient và gửi email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
