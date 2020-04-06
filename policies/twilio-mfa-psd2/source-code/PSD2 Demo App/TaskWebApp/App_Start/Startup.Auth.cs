using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using TaskWebApp.Utils;

namespace TaskWebApp
{
	public partial class Startup
	{
		/*
        * Configure the OWIN middleware
        */

		public void ConfigureAuth(IAppBuilder app)
		{
			// Required for Azure webapps, as by default they force TLS 1.2 and this project attempts 1.0
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

			app.UseCookieAuthentication(new CookieAuthenticationOptions());

			app.UseOpenIdConnectAuthentication(
				new OpenIdConnectAuthenticationOptions
				{
					// Generate the metadata address using the tenant and policy information
					MetadataAddress = String.Format(Globals.WellKnownMetadata, Globals.Tenant, Globals.DefaultPolicy),

					// These are standard OpenID Connect parameters, with values pulled from web.config
					ClientId = Globals.ClientId,
					RedirectUri = Globals.RedirectUri,
					PostLogoutRedirectUri = Globals.RedirectUri,

					// Specify the callbacks for each type of notifications
					Notifications = new OpenIdConnectAuthenticationNotifications
					{
                        RedirectToIdentityProvider = OnRedirectToIdentityProvider,

                        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
						AuthenticationFailed = OnAuthenticationFailed,
					},

					// Specify the claim type that specifies the Name property.
					TokenValidationParameters = new TokenValidationParameters
					{
						NameClaimType = "name",
						ValidateIssuer = false
					},

					// Specify the scope by appending all of the scopes requested into one string (separated by a blank space)
					Scope = $"openid profile offline_access {Globals.ReadTasksScope} {Globals.WriteTasksScope}"
				}
			);
		}

		/*
         *  On each call to Azure AD B2C, check if a policy (e.g. the profile edit or password reset policy) has been specified in the OWIN context.
         *  If so, use that policy when making the call. Also, don't request a code (since it won't be needed).
         */
		private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
		{
			var policy = notification.OwinContext.Get<string>("Policy");

            if (notification.OwinContext.Authentication.AuthenticationResponseChallenge != null)
            {
                var dict = notification.OwinContext.Authentication.AuthenticationResponseChallenge.Properties.Dictionary;
                if (dict.ContainsKey("id_token_hint"))
                {
                    notification.ProtocolMessage.IdTokenHint = dict["id_token_hint"];
                    //notification.ProtocolMessage.Prompt = "login";
                }
            }
            if (!string.IsNullOrEmpty(policy) && !policy.Equals(Globals.DefaultPolicy))
			{
				notification.ProtocolMessage.Scope = OpenIdConnectScope.OpenId;
				notification.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
				notification.ProtocolMessage.IssuerAddress = notification.ProtocolMessage.IssuerAddress.ToLower().Replace(Globals.DefaultPolicy.ToLower(), policy.ToLower());
			}

			return Task.FromResult(0);
		}


        /*
         * Catch any failures received by the authentication middleware and handle appropriately
         */
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
		{
			notification.HandleResponse();

			// Handle the error code that Azure AD B2C throws when trying to reset a password from the login page
			// because password reset is not supported by a "sign-up or sign-in policy"
			if (notification.ProtocolMessage.ErrorDescription != null && notification.ProtocolMessage.ErrorDescription.Contains("AADB2C90118"))
			{
				// If the user clicked the reset password link, redirect to the reset password route
				notification.Response.Redirect("/Account/ResetPassword");
			}
			else if (notification.Exception.Message == "access_denied")
			{
				notification.Response.Redirect("/");
			}
			else
			{
				notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
			}

			return Task.FromResult(0);
		}

		/*
         * Callback function when an authorization code is received
         */
		private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
		{
			try
			{
				/*
				 The `MSALPerUserMemoryTokenCache` is created and hooked in the `UserTokenCache` used by `IConfidentialClientApplication`.
				 At this point, if you inspect `ClaimsPrinciple.Current` you will notice that the Identity is still unauthenticated and it has no claims,
				 but `MSALPerUserMemoryTokenCache` needs the claims to work properly. Because of this sync problem, we are using the constructor that
				 receives `ClaimsPrincipal` as argument and we are getting the claims from the object `AuthorizationCodeReceivedNotification context`.
				 This object contains the property `AuthenticationTicket.Identity`, which is a `ClaimsIdentity`, created from the token received from
				 Azure AD and has a full set of claims.
				 */
				IConfidentialClientApplication confidentialClient = MsalAppBuilder.BuildConfidentialClientApplication(new ClaimsPrincipal(notification.AuthenticationTicket.Identity));

				// Upon successful sign in, get & cache a token using MSAL
				AuthenticationResult result = await confidentialClient.AcquireTokenByAuthorizationCode(Globals.Scopes, notification.Code).ExecuteAsync();
			}
			catch (Exception ex)
			{
				throw new HttpResponseException(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest,
					ReasonPhrase = $"Unable to get authorization code {ex.Message}."
				});
			}
		}
	}
}