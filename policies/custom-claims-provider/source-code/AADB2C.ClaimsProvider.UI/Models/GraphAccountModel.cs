using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AADB2C.ClaimsProvider.UI.Models
{


    public class GraphAccountModel
    {
        public string City { get; set; }
        public GraphAccountModel() { }
        
        /// <summary>
        /// Serialize the object into Json string
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GraphAccountModel Parse(string JSON)
        {
            return JsonConvert.DeserializeObject(JSON, typeof(GraphAccountModel)) as GraphAccountModel;
        }
    }

    public class PasswordProfile
    {
        public string password { get; set; }
        public bool forceChangePasswordNextLogin { get; set; }

        public PasswordProfile(string password)
        {
            this.password = password;

            // always set to false
            this.forceChangePasswordNextLogin = false;
        }
    }
    public class SignInName
    {
        public string type { get; set; }
        public string value { get; set; }

        public SignInName(string type, string value)
        {
            // Type must be 'emailAddress' (or 'userName')
            this.type = type;

            // The user email address
            this.value = value;
        }
    }

    public class UserIdentity
    {
        public string issuer { get; set; }
        public string issuerUserId { get; set; }

        public UserIdentity(string issuer, string issuerUserId)
        {
            // The identity provider name, such as facebook.com 
            this.issuer = issuer;

            // A unique user identifier for the issuer
            this.issuerUserId = issuerUserId;
        }
    }

    public class GraphUserSetPasswordModel
    {
        public PasswordProfile passwordProfile { get; }
        public string passwordPolicies { get; }

        public GraphUserSetPasswordModel(string password)
        {
            this.passwordProfile = new PasswordProfile(password);
            this.passwordPolicies = "DisablePasswordExpiration,DisableStrongPassword";
        }
    }
}
