using TaskWebApp.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace TaskWebApp.Controllers
{
    public class TokenMetadataController : Controller
    {
#if DEBUG
        const Formatting JsonFormatting = Formatting.Indented;
#else
        const Formatting JsonFormatting = Formatting.None;
#endif

        [Route(".well-known/openid-configuration", Name = "OIDC Metadata")]
        public ActionResult Metadata()

        {
            return Content(JsonConvert.SerializeObject(new OidcMetadata
            {
                Issuer = JwtConfig.JwtIssuer,
                // Include protocol param to force an absolute URL
                JwksUri = Url.RouteUrl("JWKS", null, protocol: Request.Url.Scheme),
                IdTokenSigningAlgValuesSupported = new[] { JwtConfig.JwtSigningCredentials.Algorithm },
            }, JsonFormatting), "application/json");
        }

        [Route(".well-known/jwks", Name = "JWKS")]
        public ActionResult JwksDocument()
        {
            JwksKey key = JwksKey.FromSigningCredentials(JwtConfig.JwtSigningCredentials);

            return Content(JsonConvert.SerializeObject(new JwksDocument
            {
                Keys = new[] { key }
            }, JsonFormatting), "application/json");
        }
    }
}