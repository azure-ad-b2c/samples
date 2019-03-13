using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.AADB2C.API.Models
{
    public class userAccount
    {
        public bool accountEnabled { get; set; }
        public List<SignInName>signInNames { get; set; }
        public string creationType { get; set; }
        public string displayName { get; set; }
        public string mailNickname { get; set; }
        public PasswordProfile passwordProfile { get; set; }
        public string passwordPolicies { get; set; }
        public string extension_74467a80b26148fcbaa42f7b7f335d4c_isMigrated { get; set; }
        public userAccount(string email, string password)
        {
            accountEnabled = true;
            creationType = "LocalAccount";
            PasswordProfile ps = new PasswordProfile();
            List<SignInName> sinList = new List<SignInName>();
            SignInName sin = new SignInName();
            sinList.Add(sin);
            ps.password = password;
            sin.type = "emailAddress";
            sin.value = email;
            ps.forceChangePasswordNextLogin = false;
            passwordPolicies = "DisablePasswordExpiration";
            this.passwordProfile = ps;
            this.signInNames = sinList;
            extension_74467a80b26148fcbaa42f7b7f335d4c_isMigrated = "true";
        }
    }
    public class SignInName
    {
        public string type { get; set; }
        public string value { get; set; }
    }
    public class PasswordProfile
    {
        public string password { get; set; }
        public bool forceChangePasswordNextLogin { get; set; }
    }
}