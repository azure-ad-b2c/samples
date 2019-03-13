using Contoso.AADB2C.API.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Contoso.AADB2C.API.Controllers
{
    public class IdentityController : ApiController
    {
        [HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> SignUpAsync()
        {
            // If no data came in, then return
            if (this.Request.Content == null) throw new Exception();

            // Read the input claims from the request body
            string input = Request.Content.ReadAsStringAsync().Result;

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return Content(HttpStatusCode.Conflict, new B2CResponseContent("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            InputClaimsModel inputClaims = JsonConvert.DeserializeObject(input, typeof(InputClaimsModel)) as InputClaimsModel;
            OutputClaimsModel outputClaims = new OutputClaimsModel();

            //Run API based logon against Legacy IdP, in this case it calls Azure AD's API based authentication endpoint
            using (HttpClient client = new HttpClient())
            {
                var tokenEndpoint = @"https://login.windows.net/contoso.onmicrosoft.com/oauth2/token";
                var accept = "application/json";
                client.DefaultRequestHeaders.Add("Accept", accept);
                string postBody = System.String.Format(@"resource=https%3A%2F%2Fgraph.windows.net%2F&client_id=3ce2b90b-9570-4397-a05e-edd06d6d8b65&username={0}&password={1}&scope=openid&grant_type=password", inputClaims.email, HttpContext.Current.Server.UrlEncode(inputClaims.password));
                using (var response = await client.PostAsync(tokenEndpoint, new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded")))
                {
                    //if the creds auth'd - create the user with graph api into b2c
                    if (response.IsSuccessStatusCode)
                    {
                        //Run token validation

                        outputClaims.tokenSuccess = true;
                        outputClaims.migrationRequired = false;
                        return Ok(outputClaims);
                    }
                    return Content(HttpStatusCode.Conflict, new B2CResponseContent("Migrater API - Incorrect password", HttpStatusCode.Conflict));
                }
            }

        }
    }
}
