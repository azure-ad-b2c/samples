#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    string requestContentAsString = await request.Content.ReadAsStringAsync();

    log.Info(requestContentAsString);

    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);

    if (requestContentAsJObject.appId == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    var appId = ((string) requestContentAsJObject.appId);
    string[] IDPs;

    if (appId == "999919a0-c6b0-4e74-a76f-01684b821780")
    {
        IDPs = new string[] {"facebook.com"};
    }
    else
    {
        IDPs = new string[] {"facebook.com", "google.com"};
    }

    return request.CreateResponse<ResponseContent>(
        HttpStatusCode.OK,
        new ResponseContent
        {
            version = "1.0.0",
            status = (int) HttpStatusCode.OK,
            userMessage = "List of identity providers",
            identityProviders =  IDPs
        },
        new JsonMediaTypeFormatter(),
        "application/json");
}

public class ResponseContent
{
    public string version { get; set; }

    public int status { get; set; }

    public string userMessage { get; set; }

    public string[] identityProviders {get; set; }
}

