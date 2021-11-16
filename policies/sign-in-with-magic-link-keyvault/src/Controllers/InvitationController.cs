using Microsoft.AspNetCore.Mvc;

namespace AADMagicLinks.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AADMagicLinks.Models;
    using AADMagicLinks.Utility;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class InvitationController : Controller
    {
        private readonly AppSettingsModel _appSettings;
        private readonly KeyVaultCertificateHelper _keyVaultCertificateHelper;
        private readonly IEmailSender _mailSender;
        private readonly ILogger<InvitationController> _logger;
        
        public InvitationController(IOptions<AppSettingsModel> appSettings, 
            KeyVaultCertificateHelper keyVaultCertificateHelper, 
            IEmailSender mailSender, 
            ILogger<InvitationController> logger)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _keyVaultCertificateHelper = keyVaultCertificateHelper ?? throw new ArgumentNullException(nameof(keyVaultCertificateHelper));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> ViewInvite(string state)
        {
            var msg = $"User: {User.Identity.Name} is logged in, with state: {state}";
            _logger.LogDebug(msg);
            return Content(msg);
        }

        [HttpGet]
        public async Task<IActionResult> Send(string email, string state)
        {
            var issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";
            var audience = _appSettings.B2CClientId;
            var tokenDuration = _appSettings.LinkExpiresAfterMinutes;

            // Build/compute the token that is used as an assertion value as part of the OIDC request to the B2C endpoint
            var token = await _keyVaultCertificateHelper
                .BuildSerializedIdTokenAsync(issuer, audience, tokenDuration, email)
                .ConfigureAwait(false);
            string link = $"{_appSettings.B2CRedirectUri}?id_token_hint={token}&state={state}";
            var emailHtmlTemplate = $"<h1>Welcome {email}</h1> In order to login please click on <a href='{link}'>this link</a><br/>Thanks";

            // Send the mail
            await _mailSender
                .SendEmailAsync(_appSettings.SMTPFromAddress, email, _appSettings.SMTPSubject, emailHtmlTemplate)
                .ConfigureAwait(false);
            
            _logger.LogDebug("Sent email to {0} with state: {1}", email, state);
            
            return Content($"<h1>Email Sent!</h1> (but for debug click this <a href='{link}' target='_blank'>link</a>)", "text/html");
        }
    }
}