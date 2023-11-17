using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using WebApiAutores.Dtos;

namespace WebApiAutores.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailConfigurationDto _config;
        public EmailSenderService( IConfiguration configuration) 
        { 
            _config = configuration.GetSection("EmailConfiguration").Get<EmailConfigurationDto>();
        }

        public async Task SendEmailAsync( string email, string subject, string message) 
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_config.FromName, _config.FromAddress));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var cliente = new SmtpClient())
            {
                
                await cliente.ConnectAsync(_config.SmtpServer, 
                    _config.SmtpPort, SecureSocketOptions.StartTls);

                await cliente.AuthenticateAsync(_config.SmtpUsername, _config.SmtpPassword);
                await cliente.SendAsync(emailMessage);
                await cliente.DisconnectAsync(true);

            }
        }
    }
}
