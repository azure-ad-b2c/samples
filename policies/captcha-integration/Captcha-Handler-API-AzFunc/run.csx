#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    string captcha = req.Query["captcha"];

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    captcha = captcha ?? data?.captcha;

    log.LogInformation($"RecaptchaVerify - params: captcha={captcha}");

    if ( string.IsNullOrEmpty(captcha) ) {
        var respContent = new { version = "1.0.0", status = (int)HttpStatusCode.BadRequest, userMessage = "Invalid Captcha"};
        var json = JsonConvert.SerializeObject(respContent);
        log.LogInformation(json);
        return new HttpResponseMessage(HttpStatusCode.Conflict) {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    } 
    // store your Captcha secret key in thge app config
    var captchaSecret = System.Environment.GetEnvironmentVariable("B2C_Recaptcha_Secret"); 

    HttpClient client = new HttpClient();
    var url = $"https://www.google.com/recaptcha/api/siteverify";
    var dict= new Dictionary<string, string>();
    dict.Add("secret", captchaSecret);
    dict.Add("response", captcha);

    log.LogInformation(url);

    HttpResponseMessage res = client.PostAsync(url, new FormUrlEncodedContent(dict)).Result;
    var contents = await res.Content.ReadAsStringAsync();
    client.Dispose();
    log.LogInformation("HttpStatusCode=" + res.StatusCode.ToString());
    // {                                         {
    //  "success": false,                           "success": true,
    //  "error-codes": [                            "challenge_ts": "2020-06-16T09:38:08Z",
    //      "invalid-input-secret"                  "hostname": "yourtenant.b2clogin.com"
    //  ]                                        }
    // }
    log.LogInformation("Content=" + contents);

    // return either good message that REST API expects or 409 conflict    
    if ( res.StatusCode != HttpStatusCode.OK ) {
        var respContent = new { version = "1.0.0", status = (int)HttpStatusCode.BadRequest, userMessage = "Captcha API failed"};
        var json = JsonConvert.SerializeObject(respContent);
        log.LogInformation(json);
        return new HttpResponseMessage(HttpStatusCode.Conflict) {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }

    JObject obj = JObject.Parse(contents);
    bool success = (bool)obj["success"];
    if ( success ) {
        return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(contents, System.Text.Encoding.UTF8, "application/json")
            };
    } else {
        var respContent = new { version = "1.0.0", status = (int)HttpStatusCode.BadRequest, userMessage = "Captcha failed, retry the Captcha."};
        var json = JsonConvert.SerializeObject(respContent);
        log.LogInformation(json);
        return new HttpResponseMessage(HttpStatusCode.Conflict) {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }
}
