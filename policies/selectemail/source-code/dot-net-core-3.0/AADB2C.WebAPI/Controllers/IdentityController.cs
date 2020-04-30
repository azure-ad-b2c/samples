using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AADB2C.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace AADB2C.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentityController : Controller
    {

        [HttpPost(Name = "checkEmails")]
        public async Task<ActionResult> checkEmails()
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

            //Check if the objectId parameter is presented
            if (string.IsNullOrEmpty(inputClaims.identifier))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User identifier is null or empty", HttpStatusCode.Conflict));
            }

            // return hardcoded array of emails

            string[] arrayOfEmails = new string[3];
            arrayOfEmails[0] = "test@contoso.com";
            arrayOfEmails[1] = "fake@gmail.com";
            arrayOfEmails[2] = "john@fabrikam.com";


            try
            {
                return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel(string.Empty, HttpStatusCode.OK)
                {
                    emails = arrayOfEmails
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }

    }
}