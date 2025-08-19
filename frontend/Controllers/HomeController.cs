using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using frontend.Models;
using System.Security.Claims;

namespace frontend.Controllers;

public class HomeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
        ViewBag.IsLoggedIn = !string.IsNullOrEmpty(token);

        // Optionally decode JWT to get role, name, etc.
        if (ViewBag.IsLoggedIn)
        {
            ViewBag.UserRole = JwtHelper.GetClaimValue(token??"", ClaimTypes.Role);
            ViewBag.UserName = JwtHelper.GetClaimValue(token??"", ClaimTypes.Name) ?? "Candidate";
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
