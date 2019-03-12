using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ADB2C.Lockout.Models;
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
        static ConcurrentDictionary<string, User> users = new ConcurrentDictionary<string, User>();

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
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Cannot deserialize input claims", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.signInName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Username is null or empty", HttpStatusCode.Conflict));
            }

            try
            {
                string userName = inputClaims.signInName.ToLower().Trim();
                User user = null;

                // Get the user object
                if (!users.ContainsKey(userName))
                {
                    user = new User() { Count = 1, timeStamp = DateTime.UtcNow.Ticks };
                }
                else
                {
                    user = users[userName];
                    user.Count += 1;
                }

                // If user successfully sign-in, remove the entity 
                if (!string.IsNullOrEmpty(inputClaims.objectId))
                {
                    users.TryRemove(userName, out user);
                    return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel($"Successful sign-in", HttpStatusCode.OK));
                }

                // Unlock the account and return Ok
                if ((DateTime.UtcNow.Ticks - user.timeStamp) / (10000000 * 60) > Consts.UNLOCK_AFTER)
                {
                    user.Count = 1;
                    users.AddOrUpdate(userName, user, (key, oldValue) => user);
                    return StatusCode((int)HttpStatusCode.OK, new B2CResponseModel("User is unlocked", HttpStatusCode.OK));
                }

                // Lockout the account and rais an error
                if (user.Count > Consts.LOCKOUT_AFTER)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Your account is lockout. Please try later again", HttpStatusCode.Conflict)
                    {
                        developerMessage = $"Sign-in tries: {user.Count}"
                    });
                }
                
                // Update the counter and return Ok
                users.AddOrUpdate(userName, user, (key, oldValue) => user);

                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Bad username or password", HttpStatusCode.Conflict)
                {
                    developerMessage = $"Sign-in tries: {user.Count}"
                });

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }
    }
}