using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AADB2C.MFA.AuthyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AADB2C.MFA.AuthyApp.Controllers
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


        [HttpPost(Name = "Register")]
        public async Task<ActionResult> Register()
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

            // Check input email address for local and social accounts
            if (string.IsNullOrEmpty(inputClaims.email) && string.IsNullOrEmpty(inputClaims.signInName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email address is empty", HttpStatusCode.Conflict));
            }

            // Check input phone number
            if (string.IsNullOrEmpty(inputClaims.phoneNumber))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Phone number is empty", HttpStatusCode.Conflict));
            }

            var phoneNumberMatch = Regex.Match(inputClaims.phoneNumber, @"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)(\d{1,14})$");

            if (!phoneNumberMatch.Success || phoneNumberMatch.Groups.Count != 3)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Phone number is not in the correct format", HttpStatusCode.Conflict));
            }

            using (var client = new HttpClient())
            {
                var requestUri = $"https://api.authy.com/protected/json/users/new";
                var requestForm = new Dictionary<string, string>();
                requestForm.Add("user[email]", inputClaims.GetEamilAddress());
                requestForm.Add("user[cellphone]", phoneNumberMatch.Groups[2].Value);
                requestForm.Add("user[country_code]", phoneNumberMatch.Groups[1].Value);
                var requestContent = new FormUrlEncodedContent(requestForm);
                client.DefaultRequestHeaders.Add("X-Authy-API-Key", AppSettings.Key);

                try
                {
                    var response = await client.PostAsync(requestUri, requestContent);
                    response.EnsureSuccessStatusCode();
                    dynamic responseContent = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    string authyId = "";

                    if ((bool)responseContent.success)
                    {
                        authyId = (string)responseContent.user.id;
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("An unexpected error occurred creating the Authy user.", HttpStatusCode.Conflict));
                    }

                    B2CResponseModel outputClaims = new B2CResponseModel() { authyId = authyId };

                    return Ok(outputClaims);

                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("An unexpected error occurred creating the Authy user." + ex.Message, HttpStatusCode.Conflict));
                }
            }
        }

        [HttpPost(Name = "WaitForAuthyApproval")]
        public async Task<ActionResult> WaitForAuthyApproval()
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

            // Check input email address for local and social accounts
            if (string.IsNullOrEmpty(inputClaims.email) && string.IsNullOrEmpty(inputClaims.signInName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email address is empty", HttpStatusCode.Conflict));
            }

            // Input validation
            if (string.IsNullOrEmpty(inputClaims.email) && string.IsNullOrEmpty(inputClaims.signInName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email address is empt", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.authyId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Authy Id is empty", HttpStatusCode.Conflict));
            }

            string RequestURI = string.Empty;
            using (var client = new HttpClient())
            {
                RequestURI = $"https://api.authy.com/onetouch/json/users/{inputClaims.authyId}/approval_requests";
                var postRequestForm = new Dictionary<string, string>();
                postRequestForm.Add("message", AppSettings.Message);
                //postRequestForm.Add("details[username]", inputClaims.GetEamilAddress());
                //postRequestForm.Add("logos[][res]", "default");

                //if (!string.IsNullOrEmpty(AppSettings.Logo))
                //    postRequestForm.Add("logos[][url]", AppSettings.Logo);

                //postRequestForm.Add("seconds_to_expire", AppSettings.Timeout.ToString());

                var postRequestContent = new FormUrlEncodedContent(postRequestForm);
                client.DefaultRequestHeaders.Add("X-Authy-API-Key", AppSettings.Key);
                string Responsecontent = string.Empty;

                try
                {

                    var postResponse = await client.PostAsync(RequestURI, postRequestContent);
                    Responsecontent = await postResponse.Content.ReadAsStringAsync();

                    postResponse.EnsureSuccessStatusCode();
                    dynamic postResponseContent = JsonConvert.DeserializeObject(Responsecontent);
                    string approvalRequestId = "";

                    if ((bool)postResponseContent.success)
                    {
                        approvalRequestId = (string)postResponseContent.approval_request.uuid;
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("An unexpected error occurred creating the Authy approval request.", HttpStatusCode.Conflict));

                    }

                    var isApprovalRequestApproved = false;
                    var approvalRequestQueryCount = 0;

                    while (!isApprovalRequestApproved && approvalRequestQueryCount < (AppSettings.Timeout / 3))
                    {
                        RequestURI = $"https://api.authy.com/onetouch/json/approval_requests/{approvalRequestId}";
                        var getResponse = await client.GetAsync(RequestURI);
                        Responsecontent = await getResponse.Content.ReadAsStringAsync();
                        getResponse.EnsureSuccessStatusCode();
                        dynamic getResponseContent = JsonConvert.DeserializeObject(Responsecontent);

                        if ((bool)getResponseContent.success)
                        {
                            if (((string)getResponseContent.approval_request.status).Equals("approved"))
                            {
                                isApprovalRequestApproved = true;
                            }
                            else if (((string)getResponseContent.approval_request.status).Equals("denied"))
                            {
                                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("The Authy approval request was denied.", HttpStatusCode.Conflict));
                            }
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("An unexpected error occurred polling the Authy approval request.", HttpStatusCode.Conflict));
                        }

                        await Task.Delay(3000);
                        approvalRequestQueryCount++;
                    }

                    if (!isApprovalRequestApproved)
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("The Authy approval request has timed out.", HttpStatusCode.Conflict));
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("An unexpected error occurred polling the Authy approval request. " , HttpStatusCode.Conflict));
                }
                return Ok();
            }
        }
    }
}
