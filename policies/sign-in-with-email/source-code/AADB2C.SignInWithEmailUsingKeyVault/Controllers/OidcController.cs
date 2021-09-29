using System;
using AADB2C.SignInWithEmailUsingKeyVault.Utility;
using Microsoft.AspNetCore.Mvc;

namespace AADB2C.SignInWithEmailUsingKeyVault.Controllers
{
    [ApiController]
    public class OidcController : ControllerBase
    {
        private readonly KeyVaultCertificateHelper _keyVaultCertificateHelper;

        public OidcController(KeyVaultCertificateHelper keyVaultCertificateHelper)
        {
            _keyVaultCertificateHelper = keyVaultCertificateHelper ?? throw new ArgumentNullException(nameof(keyVaultCertificateHelper));
        }

        [Route(".well-known/openid-configuration", Name = "OIDCMetadata")]
        public ActionResult Metadata()
        {
            // Sample: The issuer name is the application root path
            var issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/";
            // Sample: Include the absolute URL to JWKs endpoint
            var jwksUri = Url.Link("JWKS", null);

            var serializedResult = _keyVaultCertificateHelper.BuildSerializedOidcConfig(issuer, jwksUri);
            return Content(serializedResult, "application/json");
        }

        [Route(".well-known/keys", Name = "JWKS")]
        public ActionResult JwksDocument()
        {
            var serializedResult = _keyVaultCertificateHelper.BuildSerializedJwks();
            return Content(serializedResult, "application/json");
        }
    }
}
