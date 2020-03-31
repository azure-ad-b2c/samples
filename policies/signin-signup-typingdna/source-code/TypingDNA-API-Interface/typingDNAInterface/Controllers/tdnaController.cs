using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using typingDNAInterface.Models;

namespace typingDNAInterface.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class tdnaController : ControllerBase
    {
        [HttpPost(Name = "getuser")]
        public async Task<ActionResult> getuser()
        {
            string input = null;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }
            b2cUserModel inputClaims = b2cUserModel.Parse(input);

            string apiKey = "cb8eae96f61d90ba1f0246ceaf5e48a7";
            string apiSecret = "914bf9a71ee253fddaa3209262d387ff";
            string id = inputClaims.objectid;
            string base_url = string.Format("https://api.typingdna.com/user/{0}", id);

            var response = await getUser(apiKey, apiSecret, base_url);
            CheckUser checkUserResponse = CheckUser.FromJson(response);
            outputModel output = new outputModel(string.Empty, HttpStatusCode.OK)
            {
                success = checkUserResponse.Success,
                count = checkUserResponse.Count
            };

            return Ok(output);
        }

        // POST: api/TDNASave
        [HttpPost(Name = "save")]
        public async Task<ActionResult> Save()
            {
                string input = null;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    input = await reader.ReadToEndAsync();
                }
                b2cUserModel inputClaims = b2cUserModel.Parse(input);

                string apiKey = "cb8eae96f61d90ba1f0246ceaf5e48a7";
                string apiSecret = "914bf9a71ee253fddaa3209262d387ff";
                string id = inputClaims.objectid;
                string tp = inputClaims.typingPattern;
                string quality = "2";
                string base_url = string.Format("https://api.typingdna.com/save/{0}", id);

                var response = await saveUser(apiKey, apiSecret, base_url, tp);

            SaveUser saveUserResponse = SaveUser.FromJson(response);

            outputModel output = new outputModel(string.Empty, HttpStatusCode.OK)
            {
                status = saveUserResponse.Status
            };

            return Ok(output);
        }

        [HttpPost(Name = "Verify")]
        public async Task<ActionResult> Verify()
        {
            string input = null;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }
            b2cUserModel inputClaims = b2cUserModel.Parse(input);

            string apiKey = "cb8eae96f61d90ba1f0246ceaf5e48a7";
            string apiSecret = "914bf9a71ee253fddaa3209262d387ff";
            string id = inputClaims.objectid;
            string tp = inputClaims.typingPattern;
            int count = inputClaims.count;
            //string quality = "2";
            string base_url = string.Format("https://api.typingdna.com/verify/{0}", id);

            var response = await verifyUser(apiKey, apiSecret, base_url, tp);
            VerifyUser verifyUserResponse = VerifyUser.FromJson(response);
            bool savePattern = false;

            if (inputClaims.count > 0 && verifyUserResponse.NetScore >= 80)
            {
                savePattern = true;
            }
                       
            bool promptMFAFlag = false;
            //training model
            if (inputClaims.count <= 2)// && verifyUserResponse.NetScore < 100)
            {
                promptMFAFlag = true;
            }
            // 3 - 10 saved patterns, less than 65 score prompt mfa
            if (inputClaims.count >= 3 && verifyUserResponse.NetScore <= 50 &&  inputClaims.count <= 5 )
            {
                promptMFAFlag = true;
            }
            // 10 - 18 saved patterns, less than 70 score prompt mfa
            if (inputClaims.count > 5 && verifyUserResponse.NetScore <= 65)
            {
                promptMFAFlag = true;
            }

            outputModel output = new outputModel(string.Empty, HttpStatusCode.OK)
            {
                netscore = verifyUserResponse.NetScore,
                promptMFA = promptMFAFlag,
                saveTypingPattern = savePattern
            };

            return Ok(output);
        }


            static async Task<string> saveUser(string apiKey, string apiSecret, string base_url, string tp)
            {
                string authstring = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", apiKey, apiSecret)));
                var baseAddress = new Uri(base_url);
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authstring);
                        var data = new FormUrlEncodedContent(new[]
                            {
                            new System.Collections.Generic.KeyValuePair<string, string>("tp", tp)
                        }
                        );
                        using (var response = await httpClient.PostAsync(string.Format(base_url), data))
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            return responseData;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
            }

            static async Task<string> verifyUser(string apiKey, string apiSecret, string base_url, string tp)
            {
                string authstring = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", apiKey, apiSecret)));
                var baseAddress = new Uri(base_url);
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authstring);
                        var data = new FormUrlEncodedContent(new[]
                            {
                            new System.Collections.Generic.KeyValuePair<string, string>("tp", tp),
                            new System.Collections.Generic.KeyValuePair<string, string>("quality", "2")
                        }
                        );
                        using (var response = await httpClient.PostAsync(string.Format(base_url), data))
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            return responseData;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
            }
        static async Task<string> getUser(string apiKey, string apiSecret, string base_url)
        {
            string authstring = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", apiKey, apiSecret)));
            var baseAddress = new Uri(base_url);
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authstring);
                    using (var response = await httpClient.GetAsync(string.Format(base_url)))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }
    }
