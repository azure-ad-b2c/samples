using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
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

        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<AppSettingsModel> appSettings, IWebHostEnvironment hostingEnvironment, ITokenProvider tokenProvider, ILogger<HomeController> logger)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Message"] = String.Empty;
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email)
        {
            var issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";
            var audience = _appSettings.B2CClientId;
            var tokenDuration = _appSettings.LinkExpiresAfterMinutes;

            var token = _tokenProvider.BuildSerializedIdToken(issuer, audience, tokenDuration, email);
            string link = BuildUrl(token);

            var emailHtmlTemplate = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "Template.html"));

            var mailMessage = new MailMessage();
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress(_appSettings.SMTPFromAddress);
            mailMessage.Subject = _appSettings.SMTPSubject;
            // TODO - determine problem with String format change here
            ////mailMessage.Body = string.Format(emailHtmlTemplate, email, link);
            emailHtmlTemplate = emailHtmlTemplate.Replace("{0}", email);
            emailHtmlTemplate = emailHtmlTemplate.Replace("{1}", link);
            mailMessage.Body = emailHtmlTemplate;
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient(_appSettings.SMTPServer, _appSettings.SMTPPort);
            smtpClient.Credentials = new System.Net.NetworkCredential(_appSettings.SMTPUsername, _appSettings.SMTPPassword);
            smtpClient.EnableSsl = _appSettings.SMTPUseSSL;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMessage);

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
