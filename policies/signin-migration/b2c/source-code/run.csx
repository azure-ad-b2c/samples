#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using Newtonsoft.Json;

// the REST API called in Custom Policy <TechnicalProfile Id="UserMigrationViaLegacyIdp">
public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string UserName = req.Query["email"];
    string Password = req.Query["password"];
    string mobile = req.Query["mobile"]; // pass in mobile becuase A) if you want to do something withit in validation B) we can trace it

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    UserName = UserName ?? data?.email;
    Password = Password ?? data?.password;
    mobile = mobile ?? data?.mobile;

    log.LogInformation("Params: email=" + UserName + " password=" + Password + " mobile=" + mobile);

    // Make a login request to AWS Cognito
    // You need to have an App client in the UserPool with NO client secret 
    // and ALLOW_USER_PASSWORD_AUTH enabled
    var awsRegion = System.Environment.GetEnvironmentVariable("AWS_Region"); //
    log.LogInformation("AWS Region = " + awsRegion );
    var url = "https://cognito-idp." + awsRegion + ".amazonaws.com/";   
    var awsUserPoolClientId = System.Environment.GetEnvironmentVariable("AWS_CognitoUserPoolAppClientId"); 

    // body we pass to AWS Cognito
    var initAuthBody = new {AuthFlow = "USER_PASSWORD_AUTH", ClientId = awsUserPoolClientId,
                            AuthParameters = new {USERNAME = UserName, PASSWORD = Password }
                            };
    // serialize to string and log it
    var jsonBody = JsonConvert.SerializeObject(initAuthBody);
    log.LogInformation( url + " AWSCognitoIdentityProviderService.InitiateAuth(): body = " + jsonBody );

    // make http POST call to AWS to Auth the user. We get 200/400 back
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");
    HttpContent body = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/x-amz-json-1.1");
    HttpResponseMessage res = client.PostAsync( url, body ).Result;
    client.Dispose();
    log.LogInformation("HttpStatusCode=" + res.StatusCode.ToString());

    // return either good message that REST API expects or 409 conflict    
    if ( res.StatusCode == HttpStatusCode.OK ) {
        var goodResponse = new {tokenSuccess = true, migrationRequired = false};
        var jsonToReturn = JsonConvert.SerializeObject(goodResponse);
        log.LogInformation(jsonToReturn);
        return new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
        };
    } else {
        var respContent = new { version = "1.0.0", status = (int)HttpStatusCode.BadRequest, 
                                userMessage = "Could not verify migrated user in old system."};
        var jsonToReturn = JsonConvert.SerializeObject(respContent);
        log.LogInformation(jsonToReturn);
        return new HttpResponseMessage(HttpStatusCode.Conflict) {
                        Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
        };
    }
        
}
