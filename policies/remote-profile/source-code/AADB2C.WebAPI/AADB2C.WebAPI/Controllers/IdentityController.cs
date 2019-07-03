using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AADB2C.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AADB2C.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly AppSettingsModel AppSettings;

        // Demo: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public IdentityController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpPost(Name = "singup")]
        public async Task<ActionResult> SignUp()
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

            if (string.IsNullOrEmpty(inputClaims.objectId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User object Id is null or empty", HttpStatusCode.Conflict));
            }

            try
            {

                CloudTable table = await this.GetUsersTable(this.AppSettings.BlobStorageConnectionString);

                // Create a new user entity.
                UserTableEntity user = new UserTableEntity(inputClaims.objectId,
                        inputClaims.email,
                        inputClaims.displayName,
                        inputClaims.givenName,
                        inputClaims.surName);

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.InsertOrReplace(user);

                // Execute the insert operation.
                await table.ExecuteAsync(insertOperation);

                return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel(string.Empty, HttpStatusCode.OK)
                {
                    //emailHash = this.HashSHA1(inputClaims.email)
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }

        [HttpPost(Name = "hash")]
        public async Task<ActionResult> Hash()
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

            if (string.IsNullOrEmpty(inputClaims.email))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email address is null or empty", HttpStatusCode.Conflict));
            }

            return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel(string.Empty, HttpStatusCode.OK)
            {
                emailHash = this.HashSHA1(inputClaims.email)
            });
        }

        public string HashSHA1(string value)
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
            return sb.ToString() + "@b2c.com";

        }

        [HttpPost(Name = "singin")]
        public async Task<ActionResult> SignIn()
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

            if (string.IsNullOrEmpty(inputClaims.objectId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User object Id is null or empty", HttpStatusCode.Conflict));
            }

            try
            {

                CloudTable table = await this.GetUsersTable(this.AppSettings.BlobStorageConnectionString);

                // Create a retrieve operation that takes a customer entity.
                TableOperation retrieveOperation = TableOperation.Retrieve<UserTableEntity>(Consts.MigrationTablePartition, inputClaims.objectId.ToLower());

                // Execute the retrieve operation.
                TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                // Print the phone number of the result.
                if (retrievedResult.Result != null)
                {
                    UserTableEntity user = (UserTableEntity)retrievedResult.Result;
                    return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel(string.Empty, HttpStatusCode.OK)
                    {
                        email = user.Email,
                        displayName = user.DisplayName,
                        givenName = user.GivenName,
                        surName = user.SurName

                    });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User profile not found", HttpStatusCode.Conflict));
                }

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }

        public async Task<CloudTable> GetUsersTable(string conectionString)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(Consts.MigrationTable);

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            return table;
        }

        //[HttpPost(Name = "validate")]
        //public async Task<ActionResult> validate()
        //{
        //    string input = null;

        //    // If not data came in, then return
        //    if (this.Request.Body == null)
        //    {
        //        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
        //    }

        //    // Read the input claims from the request body
        //    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        //    {
        //        input = await reader.ReadToEndAsync();
        //    }

        //    // Check input content value
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
        //    }

        //    // Convert the input string into InputClaimsModel object
        //    InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

        //    if (inputClaims == null)
        //    {
        //        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
        //    }

        //    if (string.IsNullOrEmpty(inputClaims.email))
        //    {
        //        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("email can't be NULL or empty", HttpStatusCode.Conflict));
        //    }

        //    // Validate the email address 
        //    if (inputClaims.email.ToLower().StartsWith("test"))
        //    {
        //        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Your email address can't start with 'test'", HttpStatusCode.Conflict));
        //    }

        //    try
        //    {
        //        // Return the email in lower case format
        //        return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel(string.Empty, HttpStatusCode.OK) {
        //            email = inputClaims.email.ToLower()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
        //    }
        //}

    }
}