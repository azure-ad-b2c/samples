#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos.Table;

/*
 * When you deploy this Azure Function, you need to step out to the Console in portal.azure.com,
 * navigate to the correct function folder, then do "copy function.json function.proj", then
 * go back and select function.proj in the editor in portal.azure.com and replace the code with
 * the provided functio.proj in this github repo. Without it, the references to table storage will
 * not work
 */
public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("RemoteUserProfile" );

    string objectId =  req.Query["objectId"];   // user's objectId
    string op = req.Query["op"];                // operation: GET | SAVE
    string geo = req.Query["geo"];              // any allowed geography. You must have a config setting STORAGE_CONNECTSTRING_<geo> 
    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    objectId = objectId ?? data?.objectId;
    op = op ?? data?.op;
    geo = geo ?? data?.geo;
    string email = data?.email;
    email = email ?? data?.Email;
    if ( string.IsNullOrEmpty(op)) {
        op = "get"; // use as default
    }
    op = op.ToLower();
    if ( string.IsNullOrEmpty(geo)) {
        geo = "EU"; // use as default
    }
    geo = geo.ToUpper();
    objectId = objectId.ToLower();
    log.LogInformation("Params: op=" + op + ", objectId=" + objectId + ", geo=" + geo);

    string connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTSTRING_" + geo); //
    // just for demoing - show Azure Storage Account name where we retrieved the user from. Don't do it in prod
    string storageAccountName = null;
    string[] parts = connectionString.Split(";");
    foreach( var part in parts ) {
        if ( part.ToLower().StartsWith("accountname=") ) {
            storageAccountName = part.Split("=")[1];
        }
    }
    string tableName = "B2CRemoteUserProfile";
    string PartitionKey = "Identity";
    bool ok = false;
    UserTableEntity user = null;
    string jsonToReturn = "";
    string userMsg = "Invalid call - Missing parameters";

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
    CloudTable table = tableClient.GetTableReference(tableName);
    table.CreateIfNotExistsAsync();

    if ( !string.IsNullOrEmpty(op) && op == "get" )  { 
        log.LogInformation("get");
        user = ReadUserFromTableStorage(table, PartitionKey, objectId );
        if (user != null)
        {
            user.StorageAccountName = storageAccountName;
            ok = true;
            jsonToReturn = JsonConvert.SerializeObject( user );
            log.LogInformation("User found: " + user);
        } else {
            log.LogInformation("User not found");
            userMsg = "Unknown user";
        }
    }

    if ( !string.IsNullOrEmpty(op) && op == "save" ) {         
        email = email.ToLower();
        user = new UserTableEntity( objectId );
        user.Email = email;
        user.DisplayName = data?.DisplayName;
        user.GivenName = data?.GivenName;
        user.SurName = data?.SurName;
        user.Mobile = data?.Mobile;
        user.LoyalityId = data?.LoyalityId;
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
    user.RowKey = user.objectId;
    TableOperation insertOperation = TableOperation.InsertOrReplace(user);
    TableResult insertResult = table.ExecuteAsync(insertOperation).Result;
    if (insertResult.Result != null)
    {
        ok = true;
    }
    return ok;
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
    public string Mobile { get; set; }
    public string LoyalityId { get; set; }
    public string StorageAccountName { get; set; }
}

