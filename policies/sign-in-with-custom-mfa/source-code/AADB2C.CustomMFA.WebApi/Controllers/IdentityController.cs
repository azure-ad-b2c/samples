using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AADB2C.CustomMFA.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace AADB2C.CustomMFA.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly AppSettingsModel AppSettings;
        Random random = new Random();

        // Demo: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public IdentityController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpPost(Name = "send")]
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

            // Check input email address. If phone number is empty, the user not found in the directory. So, return a random value 
            if (string.IsNullOrEmpty(inputClaims.phoneNumber))
            {
                B2CResponseModel b2CResponseModel = new B2CResponseModel("Ok", HttpStatusCode.OK);
                b2CResponseModel.verificationCode = Guid.NewGuid().ToString();

                return Ok(b2CResponseModel);
            }

            try
            {
                int code = random.Next(1593, 9937);

                TwilioClient.Init(this.AppSettings.SMSUsername, this.AppSettings.SMSPassword);

                var message = MessageResource.Create(
                    from: new Twilio.Types.PhoneNumber(this.AppSettings.SMSFromPhoneNumber),
                    body: $"{this.AppSettings.SMSMessage}\r\n{code}",
                    to: new Twilio.Types.PhoneNumber(inputClaims.phoneNumber)
                );

                //Output claims
                B2CResponseModel b2CResponseModel = new B2CResponseModel("Ok", HttpStatusCode.OK);
                b2CResponseModel.verificationCode = code.ToString();

                return Ok(b2CResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("An unexpected error occurred. " + ex.Message, HttpStatusCode.Conflict));
            }
        }
    }
}
