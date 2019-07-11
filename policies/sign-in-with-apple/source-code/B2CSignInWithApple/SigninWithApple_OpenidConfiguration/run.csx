#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    return (ActionResult)new OkObjectResult(new {
        issuer = "https://appleid.apple.com",
        authorization_endpoint = "https://appleid.apple.com/auth/authorize",
        token_endpoint = "https://appleid.apple.com/auth/token",
        jwks_uri = "https://appleid.apple.com/auth/keys",
        id_token_signing_alg_values_supported = new [] { "RS256" }
    });
}
