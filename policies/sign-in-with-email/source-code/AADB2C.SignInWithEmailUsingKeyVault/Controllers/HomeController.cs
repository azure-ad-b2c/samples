using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AADB2C.SignInWithEmailUsingKeyVault.Models;
using AADB2C.SignInWithEmailUsingKeyVault.Utility;

namespace AADB2C.SignInWithEmailUsingKeyVault.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly KeyVaultCertificateHelper _keyVaultCertificateHelper;
        private readonly IEmailSender _mailSender;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IOptions<AppSettingsModel> appSettings, 
            IWebHostEnvironment hostingEnvironment, 
            KeyVaultCertificateHelper keyVaultCertificateHelper, 
            IEmailSender mailSender, 
            ILogger<HomeController> logger)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _keyVaultCertificateHelper = keyVaultCertificateHelper ?? throw new ArgumentNullException(nameof(keyVaultCertificateHelper));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Message"] = String.Empty;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email)
        {
            var issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";
            var audience = _appSettings.B2CClientId;
            var tokenDuration = _appSettings.LinkExpiresAfterMinutes;

            // Build/compute the token that is used as an assertion value as part of the OIDC request to the B2C endpoint
            var token = await _keyVaultCertificateHelper
                .BuildSerializedIdTokenAsync(issuer, audience, tokenDuration, email)
                .ConfigureAwait(false);
            string link = BuildUrl(token);

            // Read the HTML email template from the local file provided by and deployed with the project
            // TODO - move the actual reading of the file to a singleton/DI
            var emailHtmlTemplate = await System.IO.File
                .ReadAllTextAsync(Path.Combine(_hostingEnvironment.ContentRootPath, "Template.html"))
                .ConfigureAwait(false);
            emailHtmlTemplate = emailHtmlTemplate.Replace("{0}", email);
            emailHtmlTemplate = emailHtmlTemplate.Replace("{1}", link);

            // Send the mail
            await _mailSender
                .SendEmailAsync(_appSettings.SMTPFromAddress, email, _appSettings.SMTPSubject, emailHtmlTemplate)
                .ConfigureAwait(false);

            ViewData["Message"] = $"Email sent to {email}";

            return View();
        }

        private string BuildUrl(string token)
        {
            string nonce = Guid.NewGuid().ToString("n");

            return string.Format(
                    _appSettings.B2CSignUpUrl,
                    _appSettings.B2CTenant,
                    _appSettings.B2CPolicy,
                    _appSettings.B2CClientId,
                    Uri.EscapeDataString(_appSettings.B2CRedirectUri),
                    nonce) + "&id_token_hint=" + token;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
