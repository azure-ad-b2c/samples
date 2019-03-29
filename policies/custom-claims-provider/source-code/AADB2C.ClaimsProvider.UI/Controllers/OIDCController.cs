using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AADB2C.ClaimsProvider.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AADB2C.ClaimsProvider.UI.Controllers
{
    public class OIDCController : Controller
    {
        private readonly AppSettingsModel AppSettings;

        public OIDCController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        public IActionResult wellknown()
        {
            this.Response.ContentType = "application/json";

            ViewData["keysURL"] = this.Url.Action("keys" ,"OIDC", null, this.Request.Scheme);
            ViewData["authorization_endpoint"] = this.Url.Action("Index", "Auth", null, this.Request.Scheme);
            ViewData["token_endpoint"] = this.Url.Action("token", "Auth", null, this.Request.Scheme);
            ViewData["end_session_endpoint"] = this.Url.Action("end_session", "Auth", null, this.Request.Scheme);
            ViewData["issuer"] = this.AppSettings.Issuer;

            return View();
        }

        public IActionResult keys()
        {
            this.Response.ContentType = "application/json";
            return View();
        }
    }
}