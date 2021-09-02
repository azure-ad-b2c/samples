using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using password_history.Models;
using password_history.Services;

namespace password_history.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PassHashController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly KeyVaultManager _kvMgr;

        public PassHashController(IConfiguration config, KeyVaultManager kvm)
        {
            _config = config;
            _kvMgr = kvm;
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] PassHashModel body)
        {
            if (!Int32.TryParse(_config["PreviousPasswordCount"], out int prevPwdCount))
            {
                prevPwdCount = 3;
            }

            var result = await _kvMgr.CheckHash(body.username, body.hash, prevPwdCount);
            if (result)
            {
                await _kvMgr.SetSecret(body.username, body.hash);
                return Ok();
            }
            return BadRequest(new ResponseContent
            {
                code = "HISTORY001",
                developerMessage = $"User password found in history list of past {prevPwdCount} passwords",
                requestId = Guid.NewGuid().ToString(),
                status = 409,
                userMessage = $"You cannot reuse the past {prevPwdCount} passwords",
                version = "1.0.0"
            });
        }
    }
}