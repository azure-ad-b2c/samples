using Microsoft.Owin.Security;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskWebApp.Utils;

namespace TaskWebApp.Controllers
{
    public class AccountController : Controller
    {
        /*
         *  Called when requesting to sign up or sign in
         */
        public void SignUpSignIn(string redirectUrl)
        {
			redirectUrl = redirectUrl ?? "/";

			// Use the default policy to process the sign up / sign in flow
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });
            return;
        }

        public void StepUp(string redirectUrl, string id_token_hint)
        {
            //redirectUrl = redirectUrl ?? "/Tasks/Confirmed";
            AuthenticationProperties properties = new AuthenticationProperties();
            properties.Dictionary["id_token_hint"] = id_token_hint;
            properties.RedirectUri = "/Tasks/Confirm";
            HttpContext.GetOwinContext().Set("Policy", Globals.StepUpPolicyId);
            HttpContext.GetOwinContext().Authentication.Challenge(properties);
            return;// RedirectToAction("Confirm", "Tasks"); //(new Dictionary<string, string>() { { "id_token_hint,", id_token_hint } })
        }

        /*
         *  Called when requesting to edit a profile
         */
        public void EditProfile()
        {
            if (Request.IsAuthenticated)
            {
                // Let the middleware know you are trying to use the edit profile policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
                HttpContext.GetOwinContext().Set("Policy", Globals.EditProfilePolicyId);

                // Set the page to redirect to after editing the profile
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

                return;
            }

            Response.Redirect("/");
        }

        /*
         *  Called when requesting to reset a password
         */
        public void ResetPassword()
        {
            // Let the middleware know you are trying to use the reset password policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            HttpContext.GetOwinContext().Set("Policy", Globals.ResetPasswordPolicyId);

            // Set the page to redirect to after changing passwords
            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            return;
        }

        /*
         *  Called when requesting to sign out
         */
        public async Task SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
				await MsalAppBuilder.ClearUserTokenCache();
				IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            }
        }
	}
}