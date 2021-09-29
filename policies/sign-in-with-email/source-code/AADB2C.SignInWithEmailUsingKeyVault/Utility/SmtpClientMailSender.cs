using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using AADB2C.SignInWithEmailUsingKeyVault.Models;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public class SmtpClientMailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientMailSender(IOptions<AppSettingsModel> appSettingsOptions)
        {
            var appSettings = appSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(appSettingsOptions));

            _smtpClient = new SmtpClient(appSettings.SMTPServer, appSettings.SMTPPort);
            _smtpClient.Credentials = new System.Net.NetworkCredential(appSettings.SMTPUsername, appSettings.SMTPPassword);
            _smtpClient.EnableSsl = appSettings.SMTPUseSSL;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public async Task SendEmailAsync(string fromEmailAddress, string destinationEmailAddress, string emailSubject, string htmlContent)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmailAddress);
            mailMessage.To.Add(destinationEmailAddress);
            mailMessage.Subject = emailSubject;
            mailMessage.Body = htmlContent;
            mailMessage.IsBodyHtml = true;

            await _smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
        }
    }
}
