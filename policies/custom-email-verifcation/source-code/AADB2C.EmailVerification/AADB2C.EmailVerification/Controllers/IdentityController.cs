using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AADB2C.EmailVerification.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OtpNet;

namespace AADB2C.EmailVerification.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly AppSettingsModel AppSettings;

        // Demo: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public IdentityController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<ActionResult> Send()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.email))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email address is missing.", HttpStatusCode.Conflict));
            }

            try
            {
                Console.WriteLine($"Sending email to '{inputClaims.email}'");


                var totp = new Totp(Encoding.UTF8.GetBytes(inputClaims.email + AppSettings.TOTPSecret),
                    step: AppSettings.TOTPStep,
                    totpSize:6);

                string code = totp.ComputeTotp();

                string htmlTemplate = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Template.html"));
                 

                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(inputClaims.email);
                mailMessage.From = new MailAddress(AppSettings.SMTPFromAddress);
                mailMessage.Subject = AppSettings.SMTPSubject;
                mailMessage.Body = string.Format(htmlTemplate, inputClaims.email, code);
                mailMessage.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient(AppSettings.SMTPServer, AppSettings.SMTPPort);
                smtpClient.Credentials = new NetworkCredential(AppSettings.SMTPUsername, AppSettings.SMTPPassword);
                smtpClient.EnableSsl = AppSettings.SMTPUseSSL;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Internal error: " + ex.Message, HttpStatusCode.Conflict));
            }


            return this.Ok("Sent");
        }

        [HttpPost]
        public async Task<ActionResult> Verify()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.email))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email address is missing.", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.code))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Verification code is missing.", HttpStatusCode.Conflict));
            }

            var totp = new Totp(Encoding.UTF8.GetBytes(inputClaims.email.Trim() + AppSettings.TOTPSecret),
                    step: AppSettings.TOTPStep,
                    totpSize: 6);

            if (totp.VerifyTotp(inputClaims.code.Trim(), out long timeWindowUsed))
            {
                return Ok();
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("The verification provided is invalid or expired.", HttpStatusCode.Conflict));
            }

        }
    }
}
