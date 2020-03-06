using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AADB2C.Introspect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AADB2C.Introspect.Controllers
{
    [Produces("application/json")]
    [Route("oauth2/introspect")]
    public class IntrospectController : Controller
    {
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            if (this.Request.QueryString.HasValue & !string.IsNullOrEmpty(this.Request.Query["token"].ToString()))
            {
                string BearerToken = this.Request.Query["token"].ToString();

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.ReadJwtToken(BearerToken);
                List<System.Security.Claims.Claim> claims = ((JwtSecurityToken)token).Claims.ToList();

                Dictionary<string, string> returnClaims = new Dictionary<string, string>();

                // Add Active flag
                bool activejwt = token.ValidTo.Ticks > 0 ? (token.ValidTo > DateTime.Now) : true;
                returnClaims.Add("active", activejwt.ToString());

                if (activejwt)
                { 
                    // Only Add claims if active as per the spec
                    foreach (var item in claims)
                    {
                        returnClaims.Add(item.Type, item.Value);
                    }
                }
                

                return returnClaims;

            }

            return null;
        }
    }
}