#r "Newtonsoft.Json"

using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

// This Azure Function must be deployed with runtime .Net Core 3.1

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log )
{
    string restApiOperation = req.Query["restApiOperation"];
    string signInName = req.Query["signInName"];
    string email = req.Query["email"];
    string displayName = req.Query["displayName"];
    string givenName = req.Query["givenName"];
    string surName = req.Query["surName"];

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    log.LogInformation(requestBody);
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    restApiOperation = restApiOperation ?? data?.restApiOperation;
    signInName = signInName ?? data?.signInName;
    email = email ?? data?.email;
    displayName = displayName ?? data?.displayName;
    givenName = givenName ?? data?.givenName;
    surName = surName ?? data?.surName;

    if ( !string.IsNullOrEmpty(restApiOperation) && restApiOperation.ToLowerInvariant() == "generate"  ) {
        var (Key, IVBase64) = AesEncryptor.GenerateSymmetricEncryptionKeyIV(256);
        byte[] saltBytes = Encoding.UTF8.GetBytes( Guid.NewGuid().ToString() );
        var msg = JsonConvert.SerializeObject( new { B2CAesKeyIV = Key + "." + IVBase64, B2CEncryptionSalt = Convert.ToBase64String( saltBytes ) });
        log.LogInformation(msg);
        return new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(msg, System.Text.Encoding.UTF8, "application/json")
        };
    }

    if ( string.IsNullOrEmpty(email) ) {
        email = signInName;
    }

    // if we don't have key, IV or Salt, request them from Azure Key Vault. Then set result as envvars as a way to cache them
    string aesKeyIV = Environment.GetEnvironmentVariable("KV_AES_VALUE");
    string hashSalt = Environment.GetEnvironmentVariable("KV_HASHSALT");
    if ( string.IsNullOrEmpty(aesKeyIV) || string.IsNullOrEmpty(hashSalt)) {
        string accessToken = AcquireAccessToken(log);
        //log.LogInformation(accessToken);
        if ( string.IsNullOrEmpty(accessToken) ) {
            var errmsg = new { version = "1.0.0", status = (int)HttpStatusCode.Conflict, userMessage = "Invalid access to Azure Key Vault"};
            return new HttpResponseMessage(HttpStatusCode.Conflict) {
                Content = new StringContent( JsonConvert.SerializeObject(errmsg), System.Text.Encoding.UTF8, "application/json") };
        }
        string KeyVaultName = Environment.GetEnvironmentVariable("KV_NAME");
        // get the Hash Salt value (should be Base64)
        JObject jsonSecret = GetKeyVaultSecretVersions(log, accessToken, KeyVaultName, "B2CEncryptionSalt");
        hashSalt = jsonSecret["value"].ToString();
        Environment.SetEnvironmentVariable("KV_HASHSALT", hashSalt);
        // get the AES Key and IV. Should both be base64 separated by a "." (see generate above)
        jsonSecret = GetKeyVaultSecretVersions(log, accessToken, KeyVaultName, "B2CAesKeyIV");
        aesKeyIV = jsonSecret["value"].ToString();
        Environment.SetEnvironmentVariable("KV_AES_VALUE", aesKeyIV);
    }
    log.LogInformation("AESKeyIV: " + aesKeyIV );
    string aesKeyBase64 = aesKeyIV.Split(".")[0];
    string aesIVBase64 = aesKeyIV.Split(".")[1];
    AesEncryptor AESEnc = new AesEncryptor(aesKeyBase64, aesIVBase64);
    log.LogInformation("hashSalt: " + hashSalt );
    HashHelper hh = new HashHelper( hashSalt );

    if ( !string.IsNullOrEmpty(restApiOperation) && restApiOperation.ToLowerInvariant() == "encode"  ) {
        // you need atleast pass email during encrypt as Signup/Signin requires it
        if ( !string.IsNullOrEmpty(email) ) {
            string emailEncrypted = hh.Hash( email );
            string displayNameEncrypted = AESEnc.Encrypt( displayName );
            string givenNameEncrypted = AESEnc.Encrypt( givenName );
            string surNameEncrypted = AESEnc.Encrypt( surName );
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
        // you need atleast pass email
        string emailPlainText = AESEnc.Decrypt( email );
        string displayNamePlainText = AESEnc.Decrypt( displayName );
        string givenNamePlainText = AESEnc.Decrypt( givenName );
        string surNamePlainText = AESEnc.Decrypt( surName );
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
// We use the Azure Key Vault REST APIs so we don't need to bother about restoring nuget packages, etc.
// Get an access_token to the Azure Key Vault via client credentials. The AppID must have a KV Access Policy to access Keys + encrypt/decrypt
public static string AcquireAccessToken(ILogger log)
{
    string tenantId = Environment.GetEnvironmentVariable("KV_TENANTID");
    string clientId = Environment.GetEnvironmentVariable("KV_CLIENTID");
    string clientSecret = Environment.GetEnvironmentVariable("KV_CLIENTSECRET");
    HttpClient client = new HttpClient();
    var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
    var dict = new Dictionary<string, string>();
    dict.Add("grant_type", "client_credentials");
    dict.Add("client_id", clientId);
    dict.Add("client_secret", clientSecret);
    dict.Add("scope", "https://vault.azure.net/.default");
    dict.Add("client_info", "1");
    HttpResponseMessage res = client.PostAsync(url, new FormUrlEncodedContent(dict)).Result;
    var contents = res.Content.ReadAsStringAsync().Result;
    client.Dispose();
    if (res.StatusCode != HttpStatusCode.OK) {
        log.LogInformation( contents.ToString() );
        return null;
    }
    return JObject.Parse(contents)["access_token"].ToString();
}
// Get Azure Key VAult Secret version
private static JObject GetKeyVaultSecretVersions(ILogger log, string accessToken, string keyVaultName, string secretName)
{
    HttpClient client = new HttpClient();
    var url = $"https://{keyVaultName}.vault.azure.net/secrets/{secretName}/?api-version=7.1";
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    HttpResponseMessage res = client.GetAsync(url).Result;
    var contents = res.Content.ReadAsStringAsync().Result;
    client.Dispose();
    if (res.StatusCode != HttpStatusCode.OK) {
        log.LogInformation( contents.ToString() );
        return null;
    }
    return JObject.Parse(contents);
}
// Helper class to hash the email attributed used as signInName
public class HashHelper
{
    private byte[] saltBytes;
    private string hashName; // MD5, SHA1, SHA256, SHA384, SHA512
    public HashHelper(string saltBase64, string hashName = "SHA512") {
        saltBytes = Convert.FromBase64String(saltBase64);
        this.hashName = hashName;
    }
    private byte[] HashBytesWithSalt(byte[] value, byte[] salt) {
        byte[] saltedValue = value.Concat(salt).ToArray();
        return HashAlgorithm.Create(hashName).ComputeHash(saltedValue);
    }
    public string Hash( string text ) {
        return Convert.ToBase64String(HashBytesWithSalt(Encoding.Unicode.GetBytes(text), saltBytes) );
    }
} // cls
// Symmetric encryption using AES to keep encrypted text short in order to fit within persisted max length limits
public class AesEncryptor
{
    private Aes cipher;
    private ICryptoTransform cryptTransformEncryptor;
    private ICryptoTransform cryptTransformDecryptor;
    public AesEncryptor( string KeyBase64, string IVBase64) {
        cipher = Aes.Create();
        cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC
        cipher.Padding = PaddingMode.ISO10126;
        cipher.Key = Convert.FromBase64String(KeyBase64);
        cipher.IV = Convert.FromBase64String(IVBase64);
        cryptTransformEncryptor = cipher.CreateEncryptor();
        cryptTransformDecryptor = cipher.CreateDecryptor();
    }
    public static (string KeyBase64, string IVBase64) GenerateSymmetricEncryptionKeyIV(int keySizeBits = 256) {
        var byteArray = new byte[keySizeBits/8];
        RandomNumberGenerator.Fill(byteArray);
        var key = Convert.ToBase64String(byteArray);
        Aes cipher = Aes.Create();
        cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC
        cipher.Padding = PaddingMode.ISO10126;
        cipher.Key = Convert.FromBase64String(key);
        var IVBase64 = Convert.ToBase64String(cipher.IV);
        return (key, IVBase64);
    }
    public string Encrypt(string text) {
        if ( string.IsNullOrEmpty(text) ) return text;
        byte[] plaintext = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String( cryptTransformEncryptor.TransformFinalBlock(plaintext, 0, plaintext.Length) );
    }
    public string Decrypt(string encryptedText) {
        if ( string.IsNullOrEmpty(encryptedText) ) return encryptedText;
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        return Encoding.UTF8.GetString( cryptTransformDecryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length) );
    }
}
