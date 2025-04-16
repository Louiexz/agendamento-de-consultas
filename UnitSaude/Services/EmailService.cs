using System.Net.Mail;
using System.Net;

namespace UnitSaude.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarAsync(string destinatario, string assunto, string mensagemHtml)
        {
            var smtpHost = _configuration["EmailSettings:Servidor"]
                ?? Environment.GetEnvironmentVariable("EMAIL_SERVIDOR");
            if (smtpHost == null)
                throw new InvalidOperationException("SMTP host is not configured.");

            var smtpPortString = _configuration["EmailSettings:Porta"]
                ?? Environment.GetEnvironmentVariable("EMAIL_PORTA");
            if (!int.TryParse(smtpPortString, out var smtpPort))
                throw new InvalidOperationException("SMTP port is not configured or invalid.");

            var smtpUser = _configuration["EmailSettings:Usuario"]
                ?? Environment.GetEnvironmentVariable("EMAIL_USUARIO");
            if (smtpUser == null)
                throw new InvalidOperationException("SMTP user is not configured.");

            var smtpPass = _configuration["EmailSettings:Senha"]
                ?? Environment.GetEnvironmentVariable("EMAIL_SENHA");
            if (smtpPass == null)
                throw new InvalidOperationException("SMTP password is not configured.");

            var usarSslString = _configuration["EmailSettings:UsarSSL"]
                ?? Environment.GetEnvironmentVariable("EMAIL_SSL");
            if (!bool.TryParse(usarSslString, out var usarSsl))
                throw new InvalidOperationException("SMTP SSL setting is not configured or invalid.");

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = usarSsl,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = assunto,
                Body = mensagemHtml,
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            await client.SendMailAsync(mailMessage);
        }
    }
}
