using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AADB2C.UserInfo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AADB2C.UserInfo.Controllers
{
    [Produces("application/json")]
    [Route("api/user_info")]
    public class user_infoController : Controller
    {
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            if (this.Request.Headers["Authorization"] != "")
            {
                string BearerToken = this.Request.Headers["Authorization"].ToString().Split(" ")[1];

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.ReadJwtToken(BearerToken);
                List<System.Security.Claims.Claim> claims = ((JwtSecurityToken)token).Claims.ToList();

                Dictionary<string, string> returnClaims = new Dictionary<string, string>();

                foreach (var item in claims)
                {
                    returnClaims.Add(item.Type, item.Value);
                }
                return returnClaims;

            }

            return null;
        }
    }
}