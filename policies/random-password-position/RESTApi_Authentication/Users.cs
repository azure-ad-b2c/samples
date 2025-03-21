using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RESTApi_Authentication
{
    public class Users
    {
        private readonly ILogger<Users> _logger;

        public Users(ILogger<Users> logger)
        {
            _logger = logger;
        }

        [Function("positions")]
        public IActionResult positions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "positions")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request (Positions function).");

            try
            {
                Random _r = new Random();

                string _position1 = _r.Next(0, 9).ToString();
                string _position2 = _r.Next(0, 9).ToString();

                while (_position2 == _position1)
                { _position2 = _r.Next(0, 9).ToString(); }

                string _position3 = _r.Next(0, 9).ToString();

                while (_position3 == _position1 || _position3 == _position2)
                { _position3 = _r.Next(0, 9).ToString(); }

                var _result = new { position1 = _position1, position2 = _position2, position3 = _position3 };

                return new OkObjectResult(_result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                return new NotFoundObjectResult("");
            }
        }

        [Function("checking")]
        public async Task<IActionResult> checkingAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "checking")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request (Checking function).");

            try
            {
                string _requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var _userData = JsonConvert.DeserializeObject<UserDataModel>(_requestBody);
                var _result = new {userOK = "Success" };

                if (_userData.code1 == "2")
                {
                    if (_userData.code2 == "4")
                    {
                        if (_userData.code3 == "6")
                        {
                            return new OkObjectResult(new { userOK = "Success" });
                        }

                        return new OkObjectResult(new { userOK = "Failed" });
                    }
                    return new OkObjectResult(new { userOK = "Failed" });
                }
                return new OkObjectResult(new { userOK = "Failed" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                return new OkObjectResult(new { userOK = "Error" });
            }

        }
    }
}
