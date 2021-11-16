using AADMagicLinks.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace AADMagicLinks.Utility
{
    public class SendGridApiEmailSender : IEmailSender
    {
        private readonly SendGridClient _sendGridClient;

        public SendGridApiEmailSender(IOptions<AppSettingsModel> appSettingsOptions)
        {
            var appSettings = appSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(appSettingsOptions));

            _sendGridClient = new SendGridClient(appSettings.SMTPPassword);
        }

        public async Task SendEmailAsync(string @from, string to, string subject, string htmlContent)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress(@from),
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(new EmailAddress(to));

            var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.IsSuccessStatusCode == false)
            {
                var responseText = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException(responseText);
            }
        }
    }
}
