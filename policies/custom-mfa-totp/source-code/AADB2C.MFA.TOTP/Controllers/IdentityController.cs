using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AADB2C.MFA.TOTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using QRCoder;

namespace AADB2C.RestoreUsername.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly AppSettingsModel AppSettings;

        // Demo: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public IdentityController(IOptions<AppSettingsModel> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpPost(Name = "Generate")]
        public async Task<ActionResult> Generate()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            try
            {

                // Define the URL for the QR code. When user scan this URL, it opens one of the 
                // authentication apps running on the mobile device
                byte[] secretKey = KeyGeneration.GenerateRandomKey(20);

                string TOTPUrl = GetTotpUrl(secretKey
                    , inputClaims.userName
                    , AppSettings.TOTPIssuer
                    , AppSettings.TOTPTimestep
                    , AppSettings.TOTPAccountPrefix);

                // Generate QR code for the above URL
                var qrCodeGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(TOTPUrl, QRCodeGenerator.ECCLevel.L);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeBitmap = qrCode.GetGraphic(4);

                B2CResponseModel output = new B2CResponseModel(string.Empty, HttpStatusCode.OK)
                {
                    qrCodeBitmap = Convert.ToBase64String(qrCodeBitmap),
                    secretKey = this.EncryptAndBase64(Convert.ToBase64String(secretKey))
                };

                return Ok(output);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }

        [HttpPost(Name = "Verify")]
        public async Task<ActionResult> Verify()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            try
            {
                byte[] secretKey = Convert.FromBase64String(this.DecryptAndBase64(inputClaims.secretKey));

                Totp totp = new Totp(secretKey);
                long timeStepMatched;

                // Verify the TOTP code provided by the users
                bool verificationResult = totp.VerifyTotp(
                    inputClaims.totpCode,
                    out timeStepMatched,
                    VerificationWindow.RfcSpecifiedNetworkDelay);

                if (!verificationResult)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("The verification code is invalid.", HttpStatusCode.Conflict));
                }

                // Using the input claim 'timeStepMatched', we check whether the verification code has already been used.
                // For sign-up, the 'timeStepMatched' input claim is null and should not be evaluated 
                // For sign-in, the 'timeStepMatched' input claim contains the last time a code matched (from the user profile), and if equal to
                // the last time matched from the verify totp step, we know this code has already been used and can reject
                if (!string.IsNullOrEmpty(inputClaims.timeStepMatched) && (inputClaims.timeStepMatched).Equals(timeStepMatched.ToString()))
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("The verification code has already been used.", HttpStatusCode.Conflict));

                }

                B2CResponseModel output = new B2CResponseModel(string.Empty, HttpStatusCode.OK)
                {
                    timeStepMatched = timeStepMatched.ToString()
                };

                return Ok(output);

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel($"General error (REST API): {ex.Message}", HttpStatusCode.Conflict));
            }
        }

        public string EncryptAndBase64(string encryptString)
        {
            string EncryptionKey = this.AppSettings.EncryptionKey;
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptString));
        }

        public string DecryptAndBase64(string cipherText)
        {
            // Base64 decode
            cipherText = Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));

            string EncryptionKey = this.AppSettings.EncryptionKey;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string GetTotpUrl(byte[] key, string userName, string issuer, int timestep = 30, string prefix = null)
        {
            // if no prefix, we use the issuer
            prefix = prefix ?? issuer;

            // Escape any space, custom characters
            prefix = Uri.EscapeDataString(prefix);
            issuer = Uri.EscapeDataString(issuer);

            // Encode the key
            var secret = Base32Encoding.ToString(key);

            return string.Format("otpauth://totp/{0}:{1}?secret={2}&period={3}&issuer={0}"
                , prefix , userName, secret, timestep, issuer);
        }
    }
}
