namespace PMVInvite.Services
{
    using Microsoft.Extensions.Options;
    using Options;
    using System.Net;
    using System.Net.Mail;

    public class EmailService : IEmailService
    {
        private EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }
        
        public void SendMagicLinkEmail(string link, string email, string username)
        {
            string htmlTemplate = "<h1>Welcome {0}</h1> In order to login please click on <a href='{1}'>this link</a><br/>Thanks";

            MailMessage mailMessage = new();
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress(_emailOptions.SMTPFromAddress);
            mailMessage.Subject = _emailOptions.SMTPSubject;
            mailMessage.Body = string.Format(htmlTemplate, username, link);
            mailMessage.IsBodyHtml = true;
            using SmtpClient smtpClient = new(_emailOptions.SMTPServer, _emailOptions.SMTPPort);
            smtpClient.Credentials = new NetworkCredential(_emailOptions.SMTPUsername, _emailOptions.SMTPPassword);
            smtpClient.EnableSsl = _emailOptions.SMTPUseSSL;
            smtpClient.Send(mailMessage);
        }
    }
}