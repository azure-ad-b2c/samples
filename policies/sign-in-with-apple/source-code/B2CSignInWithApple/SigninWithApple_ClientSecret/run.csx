#r "Microsoft.IdentityModel.Tokens"
#r "Newtonsoft.Json"

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    string audience = "https://appleid.apple.com";

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    string issuer = data?.appleTeamId;
    string subject = data?.appleServiceId;
    string p8key = data?.p8key;

    IList<Claim> claims = new List<Claim> {
        new Claim ("sub", subject)
    };

    var cngKey = CngKey.Import(Convert.FromBase64String(p8key), CngKeyBlobFormat.Pkcs8PrivateBlob);

    SigningCredentials signingCred = new SigningCredentials(
        new ECDsaSecurityKey(new ECDsaCng(cngKey)),
        SecurityAlgorithms.EcdsaSha256
    );

    var token = new JwtSecurityToken(
        issuer,
        audience,
        claims,
        DateTime.Now,
        DateTime.Now.AddDays(1),
        signingCred
    );

    var tokenHandler = new JwtSecurityTokenHandler();
    
    string jwt = tokenHandler.WriteToken(token);

    return (ActionResult)new OkObjectResult(new {
        token = jwt
    });
}
