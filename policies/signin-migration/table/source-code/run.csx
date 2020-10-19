#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos.Table;

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("TableStorageUser" );

    string objectId =  req.Query["objectId"];
    string email =  req.Query["email"];
    string op = req.Query["op"];
    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    objectId = objectId ?? data?.objectId;
    op = op ?? data?.op;
    email = email ?? data?.email;
    email = email ?? data?.Email;
    if ( string.IsNullOrEmpty(op)) {
        op = "signin"; // use as default
    }
    op = op.ToLower();
    log.LogInformation("Params: objectId=" + objectId + ", op=" + op + ", email=" + email);

    string connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTSTRING"); //
    string tableName = "Users";
    string PartitionKey = "Identity";
    bool ok = false;
    UserTableEntity user = null;
    string jsonToReturn = "";
    string userMsg = "Invalid call - Missing parameters";

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
    CloudTable table = tableClient.GetTableReference(tableName);
    table.CreateIfNotExistsAsync();

    if ( !string.IsNullOrEmpty(op) && op == "getbyemail" )  { 
        email = email.ToLower();
        log.LogInformation("getbyemail");
        user = ReadUserFromTableStorage(table, PartitionKey, email );
        if (user != null)
        {
            ok = true;
            jsonToReturn = JsonConvert.SerializeObject( user );
            log.LogInformation("User found: " + user);
        } else {
            log.LogInformation("User not found");
            userMsg = "Unknown user";
        }
    }

    if ( !string.IsNullOrEmpty(op) && op == "signin" )  { 
        email = email.ToLower();
        log.LogInformation("signin");
        user = ReadUserFromTableStorage(table, PartitionKey, email );
        bool pwdMatch = false;
        if (user != null ) {
            string pwd2 = data?.password.Value;
            pwd2 = HashSHA1( pwd2 );
            pwdMatch = user.Password == pwd2;
            log.LogInformation(user.Password + " == " + pwd2 + " eq " + pwdMatch);
        }
        if (user != null && pwdMatch )
        {
            ok = true;
            jsonToReturn = JsonConvert.SerializeObject( new {tokenSuccess = true, migrationRequired = false} );
            log.LogInformation("User found and password match: " + user);
        } else {
            log.LogInformation("User not found or password not match");
            userMsg = "Unknown user or password";
        }
    }

    if ( !string.IsNullOrEmpty(op) && op == "save" ) {         
        objectId = objectId.ToLower();
        email = email.ToLower();
        user = new UserTableEntity( objectId );
        user.Email = email;
        user.DisplayName = data?.DisplayName;
        user.GivenName = data?.GivenName;
        user.SurName = data?.SurName;
        user.Password = data?.Password;
        user.Password = HashSHA1( user.Password );
        user.Mobile = data?.Mobile;
        log.LogInformation("Storing User: " + user);
        if ( WriteUserToTableStorage( table, user ) ) {
            ok = true;            
            jsonToReturn = JsonConvert.SerializeObject( user );
        } else {
            log.LogInformation("Error Storing User");
            userMsg = "Unknown user";
        }
    }

    // return either good message that REST API expects or 409 conflict    
    if ( ok ) {        
        log.LogInformation(jsonToReturn);
        return new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
        };
    } else {
        var respContent = new { version = "1.0.0", status = (int)HttpStatusCode.BadRequest, userMessage = userMsg};
        jsonToReturn = JsonConvert.SerializeObject(respContent);
        log.LogInformation(jsonToReturn);
        return new HttpResponseMessage(HttpStatusCode.Conflict) {
                        Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
        };
    }        
}

public static UserTableEntity ReadUserFromTableStorage( CloudTable table, string PartitionKey, string RowKey )
{
    UserTableEntity user = null;
    TableOperation retrieveOperation = TableOperation.Retrieve<UserTableEntity>(PartitionKey, RowKey );
    TableResult retrievedResult = table.ExecuteAsync(retrieveOperation).Result;
    if (retrievedResult.Result != null)
    {
        user = (UserTableEntity)retrievedResult.Result;
    }
    return user;
}

public static bool WriteUserToTableStorage( CloudTable table, UserTableEntity user )
{
    bool ok = false;
    user.RowKey = user.Email;
    TableOperation insertOperation = TableOperation.InsertOrReplace(user);
    TableResult insertResult = table.ExecuteAsync(insertOperation).Result;
    if (insertResult.Result != null)
    {
        ok = true;
    }
    return ok;
}

public static string HashSHA1(string value)
{
    if (string.IsNullOrEmpty(value))
        return null;
    var sha1 = SHA1.Create();
    var inputBytes = Encoding.UTF8.GetBytes(value);
    var hash = sha1.ComputeHash(inputBytes);
    var sb = new StringBuilder();
    for (var i = 0; i < hash.Length; i++)
    {
        sb.Append(hash[i].ToString("X2"));
    }
    return sb.ToString();
}

public class UserTableEntity : TableEntity
{
    public UserTableEntity()
    {
    }
    public UserTableEntity(string objectId)
    {
        this.PartitionKey = "Identity";
        this.RowKey = objectId;
        this.objectId = objectId;
    }
    public string objectId { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string GivenName { get; set; }
    public string SurName { get; set; }
    public string Password { get; set; }
    public string Mobile { get; set; }
}

