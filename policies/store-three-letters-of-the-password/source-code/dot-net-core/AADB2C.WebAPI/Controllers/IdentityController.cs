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

namespace AADB2C.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentityController : Controller
    {
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

            // Check if the password parameter is presented
            if (string.IsNullOrEmpty(inputClaims.password))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Password is null or empty", HttpStatusCode.Conflict));
            }

            // Check the password length
            if (inputClaims.password.Length < 6)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("The password provided is less than 6 characters", HttpStatusCode.Conflict));
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel(string.Empty, HttpStatusCode.OK)
                {
                    password1 = GetHashString(inputClaims.password.Substring(0, 3)),
                    password2 = GetHashString(inputClaims.password.Substring(1, 3)),
                    password3 = GetHashString(inputClaims.password.Substring(2, 3))
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }

        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}