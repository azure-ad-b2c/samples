using AADB2C.Invite.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AADB2C.Invite
{
    class Program
    {
        static public AppSettingsModel AppSettings;

        static void Main(string[] args)
        {
            // Get the app settings
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            Program.AppSettings = new AppSettingsModel();
            config.GetSection("AppSettings").Bind(Program.AppSettings);

            // Set the report type
            Console.WriteLine("1) Type the email address and then press enter:");
            string emailAddress = Console.ReadLine();

            Console.WriteLine();

            Console.WriteLine("2) Type the MFA phone number and then press enter:\r\n\tIf MFA is not required, just click enter.");
            string phoneNumber = Console.ReadLine();

            Console.WriteLine();

            SendEmail(emailAddress, phoneNumber);
        }

        public static void SendEmail(string emailAddress, string phoneNumber)
        {

            // Generate link to next step
            string client_assertion = GenerateJWTClientToken(emailAddress, phoneNumber);
            string link = string.Empty;
            string Body = string.Empty;

            string htmlTemplate = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Template.html"));

            link = AppSettings.SignUpUrl +
                $"&client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer&client_assertion={client_assertion}";

            try
            {
                Console.WriteLine($"Sending email to '{emailAddress}'");

                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(emailAddress);
                mailMessage.From = new MailAddress(AppSettings.SMTPFromAddress);
                mailMessage.Subject = AppSettings.SMTPSubject;
                mailMessage.Body = string.Format(htmlTemplate, emailAddress, link);
                mailMessage.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient(AppSettings.SMTPServer, AppSettings.SMTPPort);
                smtpClient.Credentials = new NetworkCredential(AppSettings.SMTPUsername, AppSettings.SMTPPassword);
                smtpClient.EnableSsl = AppSettings.SMTPUseSSL;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);

                Console.WriteLine("Email sent");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }


        public static string GenerateJWTClientToken(string email, string phoneNumber)
        {
            const string issuer = "http://www.contoso.com";
            const string audience = "http://azure.microsoft.com/B2C/invite";

            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim("ValidatedEmail", email, System.Security.Claims.ClaimValueTypes.String, issuer));

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                claims.Add(new System.Security.Claims.Claim("phoneNumber", phoneNumber, System.Security.Claims.ClaimValueTypes.String, issuer));
            }

            // Use the ida:ClientSigningKey value to sign the token
            // Note: This key phrase needs to be stored also in Azure B2C Keys for token validation
            var securityKey = Encoding.ASCII.GetBytes(AppSettings.ClientSigningKey);

            //TEMP
            var signingKey = new SymmetricSecurityKey(securityKey);
            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);



            // Create the signing credetails for JWT
            //SigningCredentials cred = new SigningCredentials(new InMemorySymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            // create token
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    signingCredentials);

            // Get the representation of the signed token
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            string jwtOnTheWire = jwtHandler.WriteToken(token);

            return jwtOnTheWire;
        }
    }
}
