using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AzureADB2C.Invite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AzureADB2C.Invite.Controllers
{
    public class OidcController : Controller
    {
        private static Lazy<X509SigningCredentials> SigningCredentials;
        private readonly AppSettingsModel AppSettings;
        private readonly IWebHostEnvironment HostingEnvironment;
        private readonly ILogger<HomeController> _logger;

        // Sample: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public OidcController(ILogger<HomeController> logger, IOptions<AppSettingsModel> appSettings, IWebHostEnvironment hostingEnvironment)
        {
            this.AppSettings = appSettings.Value;
            this.HostingEnvironment = hostingEnvironment;
            this._logger = logger;

            // Sample: Load the certificate with a private key (must be pfx file)
            SigningCredentials = new Lazy<X509SigningCredentials>(() =>
            {

                X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                            X509FindType.FindByThumbprint,
                                            this.AppSettings.SigningCertThumbprint,
                                            false);
                // Get the first cert with the thumb-print
                if (certCollection.Count > 0)
                {
                    return new X509SigningCredentials(certCollection[0]);
                }

                throw new Exception("Certificate not found");
            });
        }

        [Route(".well-known/openid-configuration", Name = "OIDCMetadata")]
        public ActionResult Metadata()
        {
            return Content(JsonConvert.SerializeObject(new OidcModel
            {
                // Sample: The issuer name is the application root path
                Issuer = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase.Value}/",

                // Sample: Include the absolute URL to JWKs endpoint
                JwksUri = Url.Link("JWKS", null),

                // Sample: Include the supported signing algorithms
                IdTokenSigningAlgValuesSupported = new[] { OidcController.SigningCredentials.Value.Algorithm },
            }), "application/json");
        }

        [Route(".well-known/keys", Name = "JWKS")]
        public ActionResult JwksDocument()
        {
            return Content(JsonConvert.SerializeObject(new JwksModel
            {
                Keys = new[] { JwksKeyModel.FromSigningCredentials(OidcController.SigningCredentials.Value) }
            }), "application/json");
        }
    }
}
