#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
  
    string restApiOperation = req.Query["restApiOperation"];
    string signInName = req.Query["signInName"];
    string email = req.Query["email"];
    string displayName = req.Query["displayName"];
    string givenName = req.Query["givenName"];
    string surName = req.Query["displayName"];

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    log.LogInformation(requestBody);
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    restApiOperation = restApiOperation ?? data?.restApiOperation;
    signInName = signInName ?? data?.signInName;
    email = email ?? data?.email;
    displayName = displayName ?? data?.displayName;
    givenName = givenName ?? data?.givenName;
    surName = surName ?? data?.surName;

    // Signin UX does not send email, but signInName, so when we get that we use it as email
    if ( string.IsNullOrEmpty(email) ) {
        email = signInName;
    }
    if ( !string.IsNullOrEmpty(restApiOperation) && restApiOperation.ToLowerInvariant() == "encode" ) {
        // you need atleast pass email during encrypt as Signup/Signin requires it
        if ( !string.IsNullOrEmpty(email) ) {
            string emailEncrypted = EncryptString( email );
            string displayNameEncrypted = EncryptString( displayName );
            string givenNameEncrypted = EncryptString( givenName );
            string surNameEncrypted = EncryptString( surName );
            var jsonToReturn = JsonConvert.SerializeObject( 
                new { emailPlainText = email, emailEncrypted = emailEncrypted,
                displayNamePlainText = displayName, displayNameEncrypted = displayNameEncrypted,
                givenNamePlainText = givenName, givenNameEncrypted = givenNameEncrypted,
                surNamePlainText = surName, surNameEncrypted =  surNameEncrypted 
            });
            log.LogInformation(jsonToReturn);
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
            };
        }
    } // encode

    if ( !string.IsNullOrEmpty(restApiOperation) && restApiOperation.ToLowerInvariant() == "decode" ) {
        // for decode, we decode what is passed in
        string emailPlainText = DecryptString( email );
        string displayNamePlainText = DecryptString( displayName );
        string givenNamePlainText = DecryptString( givenName );
        string surNamePlainText = DecryptString( surName );
        var jsonToReturn = JsonConvert.SerializeObject( 
            new { emailPlainText = emailPlainText, emailEncrypted = email,
            displayNamePlainText = displayNamePlainText, displayNameEncrypted = displayName,
            givenNamePlainText = givenNamePlainText, givenNameEncrypted = givenName,
            surNamePlainText = surNamePlainText, surNameEncrypted =  surName
        });
        log.LogInformation(jsonToReturn);
        return new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
        };
    } // encode

    var badContent = new { version = "1.0.0", status = (int)HttpStatusCode.Conflict, userMessage = "Bad Request"};
    return new HttpResponseMessage(HttpStatusCode.Conflict) {
                Content = new StringContent( JsonConvert.SerializeObject(badContent), System.Text.Encoding.UTF8, "application/json")
    };

}

// The below isn't really encyption/decryption. It just illustrates the point that you could do it
public static string EncryptString( string input ) {
    if ( string.IsNullOrEmpty(input) ) {
        return input;
    }
    string resp = Base64Encode(input);
    // remove trailing "="
    while( resp.EndsWith("=") ) {
        resp = resp.Substring(0, resp.Length-1 );
    }
    return resp;
}
public static string DecryptString( string input ) {
    if ( string.IsNullOrEmpty(input) ) {
        return input;
    }
    string resp = input;
    // add trailing "="
    while( (resp.Length % 4) > 0 ) {
        resp += "=";
    }
    return Base64Decode(resp);
}
public static string Base64Encode(string plainText) {
  var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
  return System.Convert.ToBase64String(plainTextBytes);
}
public static string Base64Decode(string base64EncodedData) {
  var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
  return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
}
