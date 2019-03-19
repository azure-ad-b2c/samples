using Contoso.AADB2C.API.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

            //Start migration process if we found the user and its not migrated already

            //Validate captcha blob at google
            using (HttpClient client = new HttpClient())
            {
                var validationEndpoint = @"https://www.google.com/recaptcha/api/siteverify";
                var secret = "6LdifYcUAAAAAK0dmf7lDXwhJ_4e4dn_NS_SaFbe";
                var accept = "application/json";
                client.DefaultRequestHeaders.Add("Accept", accept);
                string postBody = String.Format(@"secret={0}&response={1}", secret , HttpContext.Current.Server.UrlEncode(inputClaims.captcha));
                using (var response = await client.PostAsync(validationEndpoint, new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded")))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    content = content.Replace("-", "_");
                    //if the creds auth'd - create the user with graph api into b2c
                    if (response.IsSuccessStatusCode)
                    {
                        dynamic json = JsonConvert.DeserializeObject(content);

                        //Handling scenario where you dont want to reset captcha on failed password
                        //Will accept the same captcha blob twice
                        /*if (json.success == "true" || json.error_codes[0] == "timeout_or_duplicate")
                        {
                            return Content(HttpStatusCode.OK, new B2CResponseContent("", HttpStatusCode.OK));
                        }*/

                        //Always requires fresh Captcha if the user submits the page with an error (eg wrong password)
                        if (json.success == "true")
                        {
                            return Content(HttpStatusCode.OK, new B2CResponseContent("", HttpStatusCode.OK));
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, new B2CResponseContent("Captcha failed, retry the Captcha.", HttpStatusCode.Conflict));
                        }
                    }

                    return Content(HttpStatusCode.Conflict, new B2CResponseContent("Captcha API call failed.", HttpStatusCode.Conflict));
                }
            }

        }
    }
}
