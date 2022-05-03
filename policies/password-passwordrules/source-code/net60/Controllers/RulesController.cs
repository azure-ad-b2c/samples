using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pwdrules.Models;

namespace pwdrules.Controllers;

public class RulesController : Controller
{
    private readonly ILogger<RulesController> _logger;

    public RulesController(ILogger<RulesController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index([FromQuery] string pwdrules)
    {
        ViewData["rules"] = System.Net.WebUtility.UrlDecode(pwdrules);
        return View();
    }
}