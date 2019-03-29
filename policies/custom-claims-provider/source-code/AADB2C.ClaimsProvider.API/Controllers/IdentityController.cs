using AADB2C.ClaimsProvider.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AADB2C.ClaimsProvider.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly AppSettingsModel AppSettings;

        public IdentityController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpPost(Name = "checkappattributes")]
        public async Task<ActionResult> CheckAppAttributes()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.correlationId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'requestId' is null or empty", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.userId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'userId' is null or empty", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.ipAddress))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'ipAddress' is null or empty", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.appId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'appId' is null or empty", HttpStatusCode.Conflict));
            }

            string requestId = Guid.NewGuid().ToString();

            try
            {
                if (inputClaims.appId == "0239a9cc-309c-4d41-87f1-31288feb2e82" && string.IsNullOrEmpty(inputClaims.city))
                {
                    CloudTable table = await GetSignUpTable();
                    IdentityEntity identityEntity = new IdentityEntity("SignInRequest", requestId);
                    identityEntity.ipAddress = this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    identityEntity.userId = inputClaims.userId;
                    identityEntity.appId = inputClaims.appId;
                    identityEntity.correlationId = inputClaims.correlationId.ToString();

                    TableOperation insertOperation = TableOperation.InsertOrReplace(identityEntity);
                    await table.ExecuteAsync(insertOperation);

                    //Output claims
                    B2CResponseModel b2CResponseModel = new B2CResponseModel("Ok", HttpStatusCode.OK);
                    b2CResponseModel.collectData = true;
                    b2CResponseModel.requestId = EncryptAndBase64(requestId);

                    return Ok(b2CResponseModel);
                }
                else
                {
                    //Output claims
                    B2CResponseModel b2CResponseModel = new B2CResponseModel("Ok", HttpStatusCode.OK);
                    b2CResponseModel.collectData = false;

                    return Ok(b2CResponseModel);
                }

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel(ex.Message, HttpStatusCode.Conflict));
            }

        }

        public string EncryptAndBase64(string encryptString)
        {
            string EncryptionKey = AppSettings.EncryptionKey;
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptString));
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
    }
}
