#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("B2C Invite");

    string token = req.Query["t"];
    string email = req.Query["email"];

    string hostName = "yourtenant.b2clogin.com";
    string tenantName = "yourtenant.onmicrosoft.com";
    string nonce = Guid.NewGuid().ToString("n");

    // if we got a token, this is the redeem call. Build a B2C url and redirect
    if ( token != null ) {
        log.LogInformation("token=" + token);
        string clientId = "...guid...";                                     // this should be the client_id or your real webapp
        string redirectUri = "https%3A%2F%2Fwww.yourapp.com%2Fredirect";    // this should be the url of your real webapp
        string url = string.Format("https://{0}/{1}/B2C_1A_INV_redeem/oauth2/v2.0/authorize?client_id={2}&nonce={3}" 
                    + "&redirect_uri={4}&scope=openid&response_type=id_token&disable_cache=true&id_token_hint={5}"
                    , hostName, tenantName, clientId, nonce, redirectUri, token );
        log.LogInformation("redirectUri=" + url);
        return new RedirectResult( url, true);
    }

    // if we got an email, this is the generate link request. Call B2C and build the link to send in th e email
    if ( email != null ) {
        string displayName = req.Query["displayName"];
        displayName = string.IsNullOrEmpty(displayName) ? " " : displayName;
        log.LogInformation("email=" + email + ", displayName: " + displayName);        
        string clientId = "...guid...";                             // this can be a client_id you set up just for getting this JWT
        string redirectUri = "https%3A%2F%2Fjwt.ms";                // this should be a redirect to http://jwt.ms so we know what we get from B2C
        string url = string.Format("https://{0}/{1}/B2C_1A_INV_genlink/oauth2/v2.0/authorize?client_id={2}&nonce={3}" 
                    + "&redirect_uri={4}&scope=openid&response_type=id_token&disable_cache=true&login_hint={5}&displayName={6}"
                     , hostName, tenantName, clientId, nonce, redirectUri, email, displayName );
        log.LogInformation("url=" + url);        

        // invoke B2C and get the JWT token back automatically (no UX)
        HttpClientHandler httpClientHandler = new HttpClientHandler();
        httpClientHandler.AllowAutoRedirect = false;
        HttpClient client = new HttpClient(httpClientHandler);
        HttpResponseMessage res = client.GetAsync(url).Result;
        var contents = await res.Content.ReadAsStringAsync();
        client.Dispose();
        log.LogInformation("HttpStatusCode=" + res.StatusCode.ToString());
        log.LogInformation("Location=" + res.Headers.Location);
        log.LogInformation("Content=" + contents);

        string location = res.Headers.Location.ToString();
        string expectedRedirect = "https://jwt.ms/#id_token=";

        if ( res.StatusCode == HttpStatusCode.Redirect && location.StartsWith(expectedRedirect) ) {
            string id_token = location.Substring("https://jwt.ms/#id_token=".Length); 
            url = $"{req.Scheme}://{req.Host}{req.PathBase.Value}/api/invite?t=" + id_token;
            return (ActionResult)new OkObjectResult( url );
        } else {
            return new BadRequestObjectResult("Technical error. Please notify admin");
        }
    }
    return new BadRequestObjectResult("Bad request");
}
