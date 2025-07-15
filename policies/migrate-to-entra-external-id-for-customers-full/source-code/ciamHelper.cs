using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using Microsoft.Graph.Beta.Models.ODataErrors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace readUser
{
    public static class ciamHelper
    {
        [FunctionName("ciamHelper")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Console.WriteLine("\n" + "C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string objectId = data.objectId;
            string email = data.email;
            string password = data.password;
            string method = data.method;
            string phoneNumber = data.phoneNumber;
            string displayName = data.displayName;
            string givenName = data.givenName;
            string surName = data.surName;
            string mailNickName = data.mailNickName;
            string upn = data.upn;
            string issuerAssignedId = data.issuerUserId;
            string issuer = data.identityProvider;

            Console.WriteLine("\n" + "Object Id " + objectId + " Email " + email + " Password " + password + " Method " + method
                + " Phone number " + phoneNumber + " Display name " + displayName + " Given name " + givenName + " Surname " + surName
                + " UPN " + upn + " issuerAssignedId " + issuerAssignedId + " issuer " + issuer + " mailNickname " + mailNickName);

            // TODO: Add Entra External IDP tenant ID and name
            var tenantId = "tenant-id";
            var tenantName = "tenant-name.onmicrosoft.com";

            // ------------------------------
            // Authentication method
            // ------------------------------
            if (method == "auth")
            {
                Console.WriteLine("\n" + "----------------------------------------------");
                Console.WriteLine("\n" + "Authenticating user");

                using (var httpClient = new HttpClient())
                {
                    // Add Host header
                    // TODO: Add Entra External IDP tenant name
                    httpClient.DefaultRequestHeaders.Host = "tenant-name.ciamlogin.com";

                    // Step 1: Initiate
                    // TODO: Add Entra External IDP tenant name and client_id
                    var initiateUrl = "https://tenant-name.ciamlogin.com/tenant-name.onmicrosoft.com/oauth2/v2.0/initiate";
                    var initiateRequestBody = new Dictionary<string, string>
                    {
                        { "client_id", "16...8b" },
                        { "challenge_type", "password redirect" },
                        { "username", email }
                    };
                    var initiateContent = new FormUrlEncodedContent(initiateRequestBody);

                    Console.WriteLine("\n" + "Initiate URL: " + initiateUrl);
                    Console.WriteLine("\n" + "Initiate Request Body: " + JsonConvert.SerializeObject(initiateRequestBody));

                    var initiateResponse = await httpClient.PostAsync(initiateUrl, initiateContent);
                    var initiateResponseContent = await initiateResponse.Content.ReadAsStringAsync();

                    Console.WriteLine("\n" + "Initiate Response Status Code: " + initiateResponse.StatusCode);
                    Console.WriteLine("\n" + "Initiate Response Content: " + initiateResponseContent);

                    if (!initiateResponse.IsSuccessStatusCode)
                    {
                        return new ConflictObjectResult(new B2CResponseModel($"Failed to initiate authentication. Response: {initiateResponseContent}", HttpStatusCode.Conflict));
                    }

                    var initiateJson = JsonConvert.DeserializeObject<JObject>(initiateResponseContent);
                    var continuationToken = initiateJson["continuation_token"].ToString();

                    // Step 2: Challenge
                    // TODO: Add Entra External IDP tenant name and client_id
                    var challengeUrl = "https://tenant-name.ciamlogin.com/tenant-name.onmicrosoft.com/oauth2/v2.0/challenge";
                    var challengeRequestBody = new Dictionary<string, string>
                    {
                        { "client_id", "16...8b" },
                        { "challenge_type", "password redirect" },
                        { "continuation_token", continuationToken }
                    };
                    var challengeContent = new FormUrlEncodedContent(challengeRequestBody);

                    var challengeResponse = await httpClient.PostAsync(challengeUrl, challengeContent);
                    if (!challengeResponse.IsSuccessStatusCode)
                    {
                        return new ConflictObjectResult(new B2CResponseModel($"Failed to complete challenge.", HttpStatusCode.Conflict));
                    }

                    var challengeResponseContent = await challengeResponse.Content.ReadAsStringAsync();
                    var challengeJson = JsonConvert.DeserializeObject<JObject>(challengeResponseContent);
                    continuationToken = challengeJson["continuation_token"].ToString();

                    // Step 3: Token
                    // TODO: Add Entra External IDP tenant name and client_id
                    var tokenUrl = "https://tenant-name.ciamlogin.com/tenant-name.onmicrosoft.com/oauth2/v2.0/token";
                    var tokenRequestBody = new Dictionary<string, string>
                    {
                        { "client_id", "16...8b" },
                        { "password", password },
                        { "continuation_token", continuationToken },
                        { "grant_type", "password" },
                        { "scope", "openid offline_access" }
                    };
                    var tokenContent = new FormUrlEncodedContent(tokenRequestBody);

                    var tokenResponse = await httpClient.PostAsync(tokenUrl, tokenContent);

                    if (!tokenResponse.IsSuccessStatusCode)
                    {
                        return new ConflictObjectResult(new B2CResponseModel($"Failed to obtain token.", HttpStatusCode.Conflict));
                    }

                    // Read the response content
                    var responseContent = await tokenResponse.Content.ReadAsStringAsync();
                    var jsonObj = JsonConvert.DeserializeObject(responseContent);

                    Console.WriteLine("\n" + "jsonObject " + jsonObj.ToString());

                    return new OkObjectResult(jsonObj);
                }
            }

            else
            {
                Console.WriteLine("\n" + "method " + method);

                var scopes = new[] { "https://graph.microsoft.com/.default" };
                // Values from app registration  

                // TODO: Add GraphCallsFromB2CTenant client ID and secret
                var clientId = "a8...8d";
                var clientSecret = "LP...dk";

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };

                // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential  
                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);

                // get accessToken          
                var accessToken = await clientSecretCredential.GetTokenAsync(new Azure.Core.TokenRequestContext(scopes) { });

                //Console.WriteLine("\n" + accessToken.Token);

                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                // ------------------------------
                // Read user by objectId
                // ------------------------------
                if (objectId != null && method == "read")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "ObjectId - Reading user " + objectId);

                    var user = await graphClient.Users[objectId].GetAsync();

                    //log.LogInformation(user.ToString());
                    var logGivenName = user.GivenName;
                    var logSurName = user.Surname;
                    var logDisplayName = user.DisplayName;
                    var logEmail = user.Identities[0].IssuerAssignedId;
                    var logPhoneNumber = user.MobilePhone;
                    var logUPN = user.UserPrincipalName;

                    Console.WriteLine("\n" + "Given name " + logGivenName + " Surname " + logSurName + " Display name " + logDisplayName);
                    Console.WriteLine("Email " + logEmail + " Phone number " + logPhoneNumber + " UPN " + logUPN);

                    return new OkObjectResult(user);
                }

                // ------------------------------
                // Read user by email
                // ------------------------------
                if (email != null && method == "read")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Email - Reading user " + email);

                    // TODO: Add Entra External IDP tenant name
                    var user = await graphClient.Users.GetAsync((requestConfiguration) =>
                    {
                        requestConfiguration.QueryParameters.Filter = string.Format("identities/any(x:x/issuerAssignedId eq '{0}' " +
                            "and x/issuer eq 'tenant-name.onmicrosoft.com')  ", email);
                    });

                    //Console.WriteLine("User details " + user.ToString());                    

                    if (user != null && user.Value != null)
                    {
                        if (user.Value.Count > 1)
                        {
                            Console.WriteLine("Multiple users found." + " Count " + user.Value.Count);

                            return new ConflictObjectResult(new B2CResponseModel($"Multiple users found.", HttpStatusCode.Conflict));
                        }

                        foreach (var usr in user.Value)
                        {
                            Console.WriteLine($"ID: {usr.Id}");
                            Console.WriteLine($"Display Name: {usr.DisplayName}");
                            Console.WriteLine($"Given Name: {usr.GivenName}");
                            Console.WriteLine($"Surname: {usr.Surname}");
                            Console.WriteLine($"Email: {usr.Mail}");
                            Console.WriteLine($"User Principal Name (UPN): {usr.UserPrincipalName}");
                            Console.WriteLine("--------------------------------------------------");

                            return new OkObjectResult(usr);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No users found.");

                        return new ConflictObjectResult(new B2CResponseModel($"No users found.", HttpStatusCode.Conflict));
                    }

                    //return new OkObjectResult(user);
                }

                // ------------------------------
                // Create user
                // ------------------------------
                if (method == "createUser")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Creating user");
                    Console.WriteLine("Display name " + displayName + " email " + email);

                    // Generate a new GUID
                    Guid newGuid = Guid.NewGuid();

                    // TODO: Add tenant name
                    upn = string.Format("cpim_{0}@{1}", newGuid, tenantName);

                    var userRequestBody = new User
                    {
                        AccountEnabled = true,
                        DisplayName = displayName,
                        GivenName = givenName,
                        Surname = surName,
                        UserPrincipalName = upn,
                        Mail = email,
                        Identities = new List<ObjectIdentity>
                        {
                            new ObjectIdentity
                            {
                                SignInType = "emailAddress",
                                // TODO: Add Entra External IDP tenant name
                                Issuer = "tenant-name.onmicrosoft.com",
                                IssuerAssignedId = email,
                            },
                            new ObjectIdentity
                            {
                                SignInType = "userPrincipalName",
                                // TODO: Add Entra External IDP tenant name
                                Issuer = "tenant-name.onmicrosoft.com",
                                IssuerAssignedId = upn
                            },
                        },
                        PasswordProfile = new PasswordProfile
                        {
                            Password = password,
                            ForceChangePasswordNextSignIn = false,
                        },
                        PasswordPolicies = "DisablePasswordExpiration",
                    };

                    try
                    {
                        var result = await graphClient.Users.PostAsync(userRequestBody);

                        string stringObjectId = result.Id;

                        Console.WriteLine("\n" + "Result " + result + " stringObjectId " + stringObjectId);

                        return new OkObjectResult(result);
                    }

                    catch (Exception ex)
                    {
                        if (ex is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }
                        else
                        {
                            Console.WriteLine("\n" + "Exception " + ex);
                            Console.WriteLine("\n" + "Error creating user - account already exists ");

                            return new ConflictObjectResult(new B2CResponseModel($"This account already exists.", HttpStatusCode.Conflict));
                        }
                    }

                }

                // ------------------------------
                // Get phone
                // ------------------------------
                if (method == "getPhone")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Getting phone number" + " objectId " + objectId);

                    try
                    {
                        var result = await graphClient.Users[objectId].Authentication.PhoneMethods.GetAsync();

                        if (result.Value.Count == 0)
                        {
                            var jsonObject = new JObject();
                            jsonObject.Add("phoneNumber", "null");
                            return new OkObjectResult(jsonObject);
                        }
                        else
                        {
                            Console.WriteLine("\n" + "Phone number " + result.Value[0]);

                            return new OkObjectResult(result.Value[0]);
                        }
                    }

                    catch (Exception exception)
                    {
                        Console.WriteLine("\n" + "Exception " + exception);

                        if (exception is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }

                        return new ConflictObjectResult(new B2CResponseModel($"Error getting phone number.", HttpStatusCode.Conflict));

                    }

                }

                // ------------------------------
                // Set phone
                // ------------------------------
                if (method == "setPhone")
                {
                    try
                    {
                        Console.WriteLine("\n" + "----------------------------------------------");
                        Console.WriteLine("\n" + "Setting phone number " + phoneNumber + " objectId " + objectId);

                        var mfaRequestBody = new PhoneAuthenticationMethod
                        {
                            PhoneNumber = phoneNumber,
                            PhoneType = AuthenticationPhoneType.Mobile,
                        };

                        var enrolResult = await graphClient.Users[objectId].Authentication.PhoneMethods.PostAsync(mfaRequestBody);

                        return new OkObjectResult(enrolResult);
                    }

                    catch (Exception exception)
                    {
                        Console.WriteLine("\n" + "Exception " + exception);

                        if (exception is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }

                        return new ConflictObjectResult(new B2CResponseModel($"Error setting phone number.", HttpStatusCode.Conflict));

                    }
                }

                // ------------------------------
                // Update user (Profile Edit)
                // ------------------------------
                if (method == "update")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Updating user");
                    Console.WriteLine("Object Id " + objectId + " Display name " + displayName + " Given name " + givenName +
                        " Surname " + surName + " UPN " + upn + " Email " + email);

                    var userUpdateBody = new User
                    {
                        DisplayName = displayName,
                        GivenName = givenName,
                        Surname = surName,                        
                    };

                    try
                    {
                        var updatedUser = await graphClient.Users[objectId].PatchAsync(userUpdateBody);                        
                        return new OkObjectResult(updatedUser);
                    }

                    catch (Exception ex)
                    {
                        if (ex is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }

                        Console.WriteLine("\n" + "Exception " + ex);
                        Console.WriteLine("\n" + "Error updating user");

                        return new ConflictObjectResult(new B2CResponseModel($"Error updating user.", HttpStatusCode.Conflict));
                    }
                }

                // --------------------------------------
                // Read user by alternative security id
                // --------------------------------------
                if (method == "readAlternativeSecurityId")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Read AlternativeSecurityID");
                    Console.WriteLine("\n" + "IssuerAssignedId: " + issuerAssignedId + ", Issuer: " + issuer);

                    try
                    {
                        // Construct the filter query
                        string filterQuery = $"identities/any(c:c/issuerAssignedId eq '{issuerAssignedId}' and c/issuer eq '{issuer}')";

                        // Perform the GET request
                        var users = await graphClient.Users
                            .GetAsync((requestConfiguration) =>
                            {
                                requestConfiguration.QueryParameters.Filter = filterQuery;
                            });

                        // Access the count of users
                        Console.WriteLine("\n" + "Users found: " + users.Value.Count);

                        //users = await graphClient.Users.GetAsync();

                        // Iterate through the collection of users
                        if (users != null && users.Value != null)
                        {
                            foreach (var user in users.Value)
                            {
                                Console.WriteLine($"ID: {user.Id}");
                                Console.WriteLine($"Display Name: {user.DisplayName}");
                                Console.WriteLine($"Given Name: {user.GivenName}");
                                Console.WriteLine($"Surname: {user.Surname}");
                                Console.WriteLine($"Email: {user.Mail}");
                                Console.WriteLine($"User Principal Name (UPN): {user.UserPrincipalName}");                                
                                Console.WriteLine("--------------------------------------------------");

                                return new OkObjectResult(user);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No users found.");

                            return new OkObjectResult(users);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }
                        else
                        {
                            Console.WriteLine("\n" + "Exception: " + ex.Message);
                        }

                        return new ConflictObjectResult(new B2CResponseModel($"Error fetching social users: {ex.Message}", HttpStatusCode.Conflict));
                    }
                }

                // ---------------------------------------
                // Write user by alternative security id
                // ---------------------------------------
                if (method == "writeAlternativeSecurityId")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Write AlternativeSecurityID");
                    Console.WriteLine("\n" + "ObjectId: " + objectId + ", Issuer: " + issuer + ", IssuerAssignedId: " + issuerAssignedId);

                    // Generate a new GUID
                    Guid newGuid = Guid.NewGuid();
                    Guid passwordGuid = Guid.NewGuid();

                    // TODO: Add tenant name
                    string newUPN = string.Format("cpim_{0}@{1}", newGuid, tenantName);

                    try
                    {
                        var userRequestBody = new User
                        {
                            AccountEnabled = true,
                            DisplayName = displayName,
                            GivenName = givenName,
                            Surname = surName,
                            UserPrincipalName = newUPN,
                            Identities = new List<ObjectIdentity>
                        {
                            // UPN Identity
                            // TODO: Add Entra External IDP tenant name
                            new ObjectIdentity
                            {
                                SignInType = "userPrincipalName",
                                Issuer = "tenant-name.onmicrosoft.com",
                                IssuerAssignedId = newUPN
                            },
                            // Federated Identity
                            new ObjectIdentity
                            {
                                SignInType = "federated",
                                Issuer = issuer,
                                IssuerAssignedId = issuerAssignedId
                            }
                        },

                            PasswordProfile = new PasswordProfile
                            {
                                Password = passwordGuid.ToString(),
                                ForceChangePasswordNextSignIn = false,
                            },
                            PasswordPolicies = "DisablePasswordExpiration"
                        };

                        var result = await graphClient.Users.PostAsync(userRequestBody);

                        Console.WriteLine("\n" + "UPN and Federated identity added successfully");

                        return new OkObjectResult(result);

                    }

                    catch (Exception ex)
                    {
                        if (ex is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }
                        else
                        {
                            Console.WriteLine("\nException: " + ex.Message);
                        }

                        return new ConflictObjectResult(new B2CResponseModel($"Error adding federated identity: {ex.Message}", HttpStatusCode.Conflict));
                    }

                }

                // ------------------------------
                // Reset password
                // ------------------------------
                if (method == "resetPassword")
                {
                    Console.WriteLine("\n" + "----------------------------------------------");
                    Console.WriteLine("\n" + "Reset password " + " objectId " + objectId + " password " + password);

                    try
                    {
                        var userResetBody = new User
                        {
                            PasswordProfile = new PasswordProfile
                            {
                                Password = password,
                                ForceChangePasswordNextSignIn = false,
                            },
                            PasswordPolicies = "DisablePasswordExpiration"
                        };

                        var resetPasswordUser = await graphClient.Users[objectId].PatchAsync(userResetBody);
                        
                        Console.WriteLine("\n" + "Password reset successfully");

                        return new OkObjectResult(resetPasswordUser);
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("\n" + "Exception " + ex);
                        Console.WriteLine("\n" + "Error resetting password");

                        if (ex is ODataError odataError)
                        {
                            Console.WriteLine("\nODataError Code: " + odataError.Error.Code);
                            Console.WriteLine("\nODataError Message: " + odataError.Error.Message);
                        }

                        return new ConflictObjectResult(new B2CResponseModel($"Error resetting password.", HttpStatusCode.Conflict));
                    }
                }

            }

            return new OkObjectResult(null);
        }
        //public static async Task EnrolEmail(GraphServiceClient graphClient, string email, string objectId)
        //{

        //    Console.WriteLine("\n" + "Enrolling email address:" + " email " + email + " objectId " + objectId);

        //    var emailAuthMethodRequestBody = new EmailAuthenticationMethod
        //    {
        //        EmailAddress = email
        //    };

        //    var result = await graphClient.Users[objectId].Authentication.EmailMethods.PostAsync(emailAuthMethodRequestBody);

        //    Console.WriteLine("\n" + "result " + result);
        //   return new OkObjectResult(enrolResult);
        //}

        //public static async Task DoWithRetryAsync(TimeSpan sleepPeriod, int tryCount = 3, string objectId="test", string email="test", GraphServiceClient graphClient=null)
        //public static async Task DoWithRetryAsync(TimeSpan sleepPeriod, int tryCount, string objectId, string email,
        //    GraphServiceClient graphClient)
        //{
        //    Console.WriteLine("\n" + "DoWithRetryAsync");
        //    Console.WriteLine("\n" + "objectId " + objectId + " email " + email);

        //    if (tryCount <= 0)
        //        throw new ArgumentOutOfRangeException(nameof(tryCount));

        //    while (true)
        //    {
        //        try
        //        {
        //            await EnrolEmail(graphClient, email, objectId);
        //            return;
        //        }
        //        catch
        //        {
        //            if (--tryCount == 0)
        //                throw;
        //            await Task.Delay(sleepPeriod);
        //        }
        //    }
        //}
    }

    public class B2CResponseModel
    {
        public string version { get; set; }
        public int status { get; set; }
        public string userMessage { get; set; }

        public B2CResponseModel(string message, HttpStatusCode status)
        {
            Console.WriteLine("\n" + "B2C response " + message);

            this.userMessage = message;
            this.status = (int)status;
            this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
