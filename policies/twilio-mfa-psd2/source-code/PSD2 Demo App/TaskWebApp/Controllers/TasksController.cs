using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using TaskWebApp.Models;
using TaskWebApp.Utils;

namespace TaskWebApp.Controllers
{
	[Authorize]
    public class TasksController : Controller
    {
        private String apiEndpoint = Globals.ServiceUrl + "/api/tasks/";

        // GET: Makes a call to the API and retrieves the list of tasks
        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> Confirm()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;
            string acr = claim.SingleOrDefault(c => c.Type == "http://schemas.microsoft.com/claims/authnclassreference").Value;
            paymentModel paymentConfirmedDetails = new paymentModel();
            if (acr == "b2c_1a_psd2_stepup")
            {
            paymentConfirmedDetails.Payee = claim.SingleOrDefault(c => c.Type == "payee").Value;
            paymentConfirmedDetails.AccountNumber = claim.SingleOrDefault(c => c.Type == "accountnumber").Value;
            paymentConfirmedDetails.Amount = claim.SingleOrDefault(c => c.Type == "amount").Value;
            }
            if (acr != "b2c_1a_psd2_stepup")
            {
                paymentConfirmedDetails.Payee ="NA";
                paymentConfirmedDetails.AccountNumber = "NA";
                paymentConfirmedDetails.Amount = "NA";
            }

            return View("Confirm", paymentConfirmedDetails);
        }

        // POST: Makes a call to the API to store a new task
        [HttpPost]
        public RedirectToRouteResult Create(paymentModel model)
        {
            // Cast to ClaimsIdentity.
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;

            //get policyid
            var acr =  claim.SingleOrDefault(c => c.Type == "http://schemas.microsoft.com/claims/authnclassreference").Value;
            //{http://schemas.microsoft.com/claims/authnclassreference: b2c_1a_psd2_susi}

                var payee = model.Payee;
                var accountNumber = model.AccountNumber;
                var amount = model.Amount;

                Dictionary<string, string> claims = new Dictionary<string, string>();
                claims["amount"] = amount;
                claims["payee"] = payee;
                claims["accountnumber"] = accountNumber;

                string id_token_hint = BuildIdToken(claims, Globals.ClientId);

                    return RedirectToAction("StepUp", "Account", new { value1 = "", id_token_hint = id_token_hint });



        }

        [HttpPost]
        public RedirectToRouteResult Confirmed()
        {
            return RedirectToAction("SignUpSignIn", "Account");
        }

        /*
         * Helper function for returning an error message
         */
        private ActionResult ErrorAction(String message)
        {
            return new RedirectResult("/Error?message=" + message);
        }

        private string BuildIdToken(IDictionary<string, string> claimsDict, string audience)
        {
            // All parameters sent to Azure AD B2C need to be sent as claims
            IEnumerable<Claim> claims = claimsDict.Select(kvp => new Claim(kvp.Key, kvp.Value, ClaimValueTypes.String, JwtConfig.JwtIssuer));

            // Create the token
            JwtSecurityToken token = new JwtSecurityToken(
                    JwtConfig.JwtIssuer,
                    audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddDays(JwtConfig.JwtExpirationDays),
                    JwtConfig.JwtSigningCredentials);

            // Get the representation of the signed token
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            return jwtHandler.WriteToken(token);
        }

    }
}