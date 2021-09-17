using System;
using Microsoft.AspNetCore.Mvc;
using AADB2C.SignInWithEmailUsingKeyVault.Utility;

namespace AADB2C.SignInWithEmailUsingKeyVault.Controllers
{
    [ApiController]
    public class OidcController : ControllerBase
    {
        private readonly ITokenValidationConfigurationProvider _tokenValidationConfigurationProvider;

        public OidcController(ITokenValidationConfigurationProvider tokenValidationConfigurationProvider)
        {
            _tokenValidationConfigurationProvider = tokenValidationConfigurationProvider ?? throw new ArgumentNullException(nameof(tokenValidationConfigurationProvider));
        }

        [Route(".well-known/openid-configuration", Name = "OIDCMetadata")]
        public ActionResult Metadata()
        {
            // Sample: The issuer name is the application root path
            var issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";
            // Sample: Include the absolute URL to JWKs endpoint
            var jwksUri = Url.Link("JWKS", null);

            var serializedResult = _tokenValidationConfigurationProvider.BuildSerializedOidcConfig(issuer, jwksUri);
            return Content(serializedResult, "application/json");
        }

        [Route(".well-known/keys", Name = "JWKS")]
        public ActionResult JwksDocument()
        {
            var serializedResult = _tokenValidationConfigurationProvider.BuildSerializedJwks();
            return Content(serializedResult, "application/json");
        }
    }
}
