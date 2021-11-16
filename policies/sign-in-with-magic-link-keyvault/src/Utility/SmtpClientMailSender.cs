using System;
using System.Net.Mail;
using System.Threading.Tasks;
using AADMagicLinks.Models;
using Microsoft.Extensions.Options;

namespace AADMagicLinks.Utility
{
    /// <summary>
    /// Demonstrates using an <see cref="SmtpClient" instance to send mail./>
    /// </summary>
    /// <remarks>
    /// Not that <see cref="SmtpClient"/> is now deprecated and should not be used for new development. 
    /// See https://docs.microsoft.com/dotnet/api/system.net.mail.smtpclient#remarks for details
    /// </remarks>
    public class SmtpClientMailSender : IEmailSender
    {
        private readonly AppSettingsModel _appSettings;

        public SmtpClientMailSender(IOptions<AppSettingsModel> appSettingsOptions)
        {
            _appSettings = appSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(appSettingsOptions));
        }

        public async Task SendEmailAsync(string @from, string to, string subject, string htmlContent)
        {
            using (var smtpClient = new SmtpClient(_appSettings.SMTPServer, _appSettings.SMTPPort))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(_appSettings.SMTPUsername, _appSettings.SMTPPassword);
                smtpClient.EnableSsl = _appSettings.SMTPUseSSL;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(@from);
                    mailMessage.To.Add(to);
                    mailMessage.Subject = subject;
                    mailMessage.Body = htmlContent;
                    mailMessage.IsBodyHtml = true;

                    await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                }
            }
        }
    }
}
