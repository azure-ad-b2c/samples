using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AADB2C.RBAC.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AADB2C.RBAC.Sample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly AppSettingsModel AppSettings;

        // Demo: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public IdentityController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpPost(Name = "IsMemberOf")]
        public async Task<ActionResult> IsMemberOf()
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

            if (string.IsNullOrEmpty(inputClaims.objectId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'objectId' is null or empty", HttpStatusCode.Conflict));
            }


            try
            {
                AzureADGraphClient azureADGraphClient = new AzureADGraphClient(this.AppSettings.Tenant, this.AppSettings.ClientId, this.AppSettings.ClientSecret);

                // Demo: Get user's groups
                GraphGroupsModel groups = await azureADGraphClient.GetUserGroup(inputClaims.objectId);

                // Demo: Add the groups to string collections
                List<string> groupsList = new List<string>();
                foreach (var item in groups.value)
                {
                    groupsList.Add(item.displayName);
                }

                // Demo: Set the output claims
                OutputClaimsModel output = new OutputClaimsModel() { groups = groupsList };

                // Demo: Check if user needs to be a member of a security group
                if (!string.IsNullOrEmpty(inputClaims.onlyMembersOf))
                {
                    List<string> onlyMembersOf = inputClaims.onlyMembersOf.ToLower().Split(',').ToList<string>();
                    bool isMemberOf = false;
                    foreach (var item in output.groups)
                    {
                        if (onlyMembersOf.Contains(item.ToLower()))
                        {
                            isMemberOf = true;
                            break;
                        }
                    }

                    // Demo: Throw error if user is not member of one of the security groups
                    if (isMemberOf == false)
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("You are not authorized to sign-in to this application.", HttpStatusCode.Conflict));
                    }
                }

                // Demo: Return the groups collection
                return Ok(output);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Request_ResourceNotFound"))
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not read user groups, user not found", HttpStatusCode.Conflict));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not read user groups", HttpStatusCode.Conflict));
            }

        }

    }
}
