using AADB2C.RBAC.Sample.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AADB2C.RBAC.Sample
{
    public class AzureADGraphClient
    {
        private AuthenticationContext authContext;
        private ClientCredential credential;
        static private AuthenticationResult AccessToken;

        public readonly string aadInstance = "https://login.microsoftonline.com/";
        public readonly string aadGraphResourceId = "https://graph.windows.net/";
        public readonly string aadGraphEndpoint = "https://graph.windows.net/";
        public readonly string aadGraphVersion = "api-version=1.6";

        public string Tenant { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        public AzureADGraphClient(string tenant, string clientId, string clientSecret)
        {
            this.Tenant = tenant;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;

            // The AuthenticationContext is ADAL's primary class, in which you indicate the direcotry to use.
            this.authContext = new AuthenticationContext("https://login.microsoftonline.com/" + this.Tenant);

            // The ClientCredential is where you pass in your client_id and client_secret, which are 
            // provided to Azure AD in order to receive an access_token using the app's identity.
            this.credential = new ClientCredential(this.ClientId, this.ClientSecret);
        }

        /// <summary>
        /// Return 
        /// </summary>
        /// <param name="OID"></param>
        /// <returns></returns>
        public async Task<GraphGroupsModel> GetUserGroup(string OID)
        {

            if (string.IsNullOrEmpty(OID))
                throw new Exception("User object Id is null or empty");

            string JSON = await SendGraphRequest($"/users/{OID}/memberOf",
                            null,
                            null, HttpMethod.Get);

            GraphGroupsModel groups = GraphGroupsModel.Parse(JSON);

            return groups;
        }

        /// <summary>
        /// Handle Graph user API, support following HTTP methods: GET, POST and PATCH
        /// </summary>
        private async Task<string> SendGraphRequest(string api, string query, string data, HttpMethod method)
        {
            // Get the access toke to Graph API
            string acceeToken = await AcquireAccessToken();

            // Set the Graph url. Including: Graph-endpoint/tenat/users?api-version&query
            string url = $"{this.aadGraphEndpoint}{this.Tenant}{api}?{this.aadGraphVersion}";

            if (!string.IsNullOrEmpty(query))
            {
                url += "&" + query;
            }

            try
            {
                using (HttpClient http = new HttpClient())
                using (HttpRequestMessage request = new HttpRequestMessage(method, url))
                {
                    // Set the authorization header
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acceeToken);

                    // For POST and PATCH set the request content 
                    if (!string.IsNullOrEmpty(data))
                    {
                        //Trace.WriteLine($"Graph API data: {data}");
                        request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    }

                    // Send the request to Graph API endpoint
                    using (HttpResponseMessage response = await http.SendAsync(request))
                    {
                        string error = await response.Content.ReadAsStringAsync();

                        // Check the result for error
                        if (!response.IsSuccessStatusCode)
                        {
                            // Throw server busy error message
                            if (response.StatusCode == (HttpStatusCode)429)
                            {
                                // TBD: Add you error handling here
                            }

                            throw new Exception(error);
                        }

                        // Return the response body, usually in JSON format
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception)
            {
                // TBD: Add you error handling here
                throw;
            }
        }

        private async Task<string> AcquireAccessToken()
        {

            AzureADGraphClient.AccessToken = await authContext.AcquireTokenAsync(this.aadGraphResourceId, credential);

            // If the access token is null or about to be invalid, acquire new one
            //if (B2CGraphClient.AccessToken == null ||
            //    (B2CGraphClient.AccessToken.ExpiresOn.UtcDateTime > DateTime.UtcNow.AddMinutes(-10)))
            //{
            //    try
            //    {
            //        B2CGraphClient.AccessToken = await authContext.AcquireTokenAsync(this.aadGraphResourceId, credential);
            //    }
            //    catch (Exception ex)
            //    {
            //        // TBD: Add you error handling here
            //        throw;
            //    }
            //}

            return AzureADGraphClient.AccessToken.AccessToken;
        }

    }
}
