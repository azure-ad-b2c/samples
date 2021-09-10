#r "Newtonsoft.Json"

using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("SignInName lookup Function.");

    // Initialise sample objectIds
    var users = new Dictionary<string, string> {
        { "username1@mydomain.com", "d8990a4d-de05-423d-8ee6-6ff96cd98bfc@yourtenant.onmicrosoft.com" },
        { "username2@mydomain.com", "d8990a4d-de05-423d-8ee6-6ff96cd98bfc@yourtenant.onmicrosoft.com" },
        { "username3@mydomain.com", "d8990a4d-de05-423d-8ee6-6ff96cd98bfc@yourtenant.onmicrosoft.com" },
        { "username4@mydomain.com", "e7b5d18b-539b-4279-9934-02f78f448711@yourtenant.onmicrosoft.com" },        
        { "username5@mydomain.com", "e7b5d18b-539b-4279-9934-02f78f448711@yourtenant.onmicrosoft.com" }
    };

    // get the username as an query paramater or request body
    // parse query parameter
    string username = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "username", true) == 0)
        .Value;

    if (username == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        username = data?.username;
    }

    if (username == null || !users.ContainsKey(username))
    {
        dynamic errMessage = new JObject() {
        { "Version", "1.0.0"},
        { "status", "400"},
        { "userMessage", "Not a valid user."}
        };

        var ErrorResp = req.CreateResponse(HttpStatusCode.BadRequest);
        ErrorResp.Content = new StringContent(JsonConvert.SerializeObject(errMessage), Encoding.UTF8, "application/json");

        return ErrorResp;
    }

    dynamic successMessage = new JObject();
    successMessage.username = username;
    successMessage.upn = users[username];

    var successResp = req.CreateResponse(HttpStatusCode.OK);
    successResp.Content = new StringContent(JsonConvert.SerializeObject(successMessage), Encoding.UTF8, "application/json");
    
    log.Info("SignInName " + users[username] + " returned for " + username ) ;
    return successResp;

}
