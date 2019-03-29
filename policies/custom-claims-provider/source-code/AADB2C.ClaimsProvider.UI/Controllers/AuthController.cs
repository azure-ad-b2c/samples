using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AADB2C.ClaimsProvider.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AADB2C.ClaimsProvider.UI.Controllers
{
    //[Produces("application/json")]
    //[Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly AppSettingsModel AppSettings;

        public AuthController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        /// <summary>
        /// Renders the user profile form
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewData["rid"] = this.Request.Query["rid"].ToString();
            ViewData["redirect_uri"] = this.Request.Query["redirect_uri"];
            ViewData["state"] = this.Request.Query["state"];

            IdentityEntity identityEntity = await GetIdentityEntity(Request.Query["rid"]);

            // Check the request id
            if (!isValidRequest(identityEntity, this.Request.Query["redirect_uri"]))
                return View("Error");

            return View();
        }

        /// <summary>
        /// Updates the user profile, using Graph API and deleted the Azure Blob storage entity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(UserProfileModel userProfile)
        {
            string id_token = GenerateJWTClientToken(userProfile);
            string redirect_uri = this.Request.Query["redirect_uri"];
            string state = this.Request.Query["state"];
            string rid = this.Request.Query["rid"];

            IdentityEntity identityEntity = await GetIdentityEntity(rid);

            // Check the request
            if (!isValidRequest(identityEntity, redirect_uri))
                return View("Error");

            // Update the account
            AzureADGraphClient azureADGraphClient = new AzureADGraphClient(this.AppSettings.Tenant, this.AppSettings.ClientId, this.AppSettings.ClientSecret);

            // Create the user using Graph API
            await azureADGraphClient.UpdateAccount(identityEntity.userId, userProfile.City);

            // Wait until user is updated
            //await Task.Delay(2500);

            // Delete the entity
            await DeleteIdentityEntity(identityEntity);

            string redirectUri = $"{redirect_uri}?id_token={id_token}&state={state}";

            return Redirect(redirectUri);
        }

        public string DecryptAndBase64(string cipherText)
        {
            // Base64 decode
            cipherText = Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));

            string EncryptionKey = AppSettings.EncryptionKey;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /// <summary>
        /// Validates the request 
        /// </summary>
        private bool isValidRequest(IdentityEntity identityEntity, string redirect_uri)
        {
            // Check:
            //    1) request is existed in Azure Blob storage
            //    2) IP source
            //    3) redirect URL is the one registered in the app settings
            return identityEntity != null &&
                (this.Request.HttpContext.Connection.RemoteIpAddress.ToString() != identityEntity.ipAddress) &&
                (redirect_uri.ToLower().StartsWith(AppSettings.AuthorizedRedirectUri.ToLower()));
        }

        private async Task<IdentityEntity> GetIdentityEntity(string requestId)
        {
            // Decrypt the request ID
            requestId = DecryptAndBase64(requestId);

            CloudTable table = await GetSignUpTable();
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<IdentityEntity>("SignInRequest", requestId);

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            // Check if request exists 
            if (retrievedResult.Result != null)
            {

                return (IdentityEntity)retrievedResult.Result;

            }
            else
            {
                return null;
            }
        }

        private async Task DeleteIdentityEntity(IdentityEntity identityEntity)
        {
            CloudTable table = await GetSignUpTable();

            // Delete the entity
            TableOperation deleteOperation = TableOperation.Delete(identityEntity);
            await table.ExecuteAsync(deleteOperation);
        }

        private async Task<CloudTable> GetSignUpTable()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.AppSettings.BlobStorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("ClaimsProvider");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            return table;
        }

        public string GenerateJWTClientToken(UserProfileModel userProfile)
        {
            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim("sub", "1@test.com", System.Security.Claims.ClaimValueTypes.String, AppSettings.Issuer));
            claims.Add(new System.Security.Claims.Claim("name", "Yoel 111", System.Security.Claims.ClaimValueTypes.String, AppSettings.Issuer));
            claims.Add(new System.Security.Claims.Claim("city", userProfile.City, System.Security.Claims.ClaimValueTypes.String, AppSettings.Issuer));

            string dataDir = AppDomain.CurrentDomain.GetData("CertificateDir").ToString();

            X509Certificate2 cert = new X509Certificate2(
                Path.Combine(dataDir, AppSettings.CertificateName),
                AppSettings.CertificatePassword,
                X509KeyStorageFlags.DefaultKeySet);

            // create token
            JwtSecurityToken token = new JwtSecurityToken(
                    AppSettings.Issuer,
                    AppSettings.Audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddYears(1),
                    new X509SigningCredentials(cert));

            // Get the representation of the signed token
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            string jwtOnTheWire = jwtHandler.WriteToken(token);

            return jwtOnTheWire;
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }
    }
}

/*
 public X509Certificate2 LoadCertificate()
{
    if (_environment.IsDevelopment())
    {
        return new X509Certificate2(Path.Combine(_environment.ContentRootPath, "your.development.certificate.pfx"), "secret");
    }
    else
    {
        X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        certStore.Open(OpenFlags.ReadOnly);

        X509Certificate2Collection certCollection = certStore
            .Certificates
            .Find(X509FindType.FindByThumbprint,
            "XXXX-XXXX-XXXX-XXXX", // Generated by Azure
            false);

        if (certCollection.Count > 0)
        {
            X509Certificate2 cert = certCollection[0];

            return cert;
        }
        certStore.Dispose();

        return null;
    }
}
*/
