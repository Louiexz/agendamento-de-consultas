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
            var smtpHost = _configuration["EmailSettings:Servidor"];
            var smtpPort = int.Parse(_configuration["EmailSettings:Porta"]);
            var smtpUser = _configuration["EmailSettings:Usuario"];
            var smtpPass = _configuration["EmailSettings:Senha"];
            var usarSsl = bool.Parse(_configuration["EmailSettings:UsarSSL"]);

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
