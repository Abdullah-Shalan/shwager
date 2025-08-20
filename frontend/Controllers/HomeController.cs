using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using frontend.Models;
using System.Security.Claims;
using frontend.Models.Requests;

namespace frontend.Controllers;

public class HomeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var jobs = await client.GetFromJsonAsync<List<JobSummaryDto>>("/api/Hr/jobs");
        
        ViewBag.AvailableJobs = jobs;

        var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
        ViewBag.IsLoggedIn = !string.IsNullOrEmpty(token);

        // Optionally decode JWT to get role, name, etc.
        if (ViewBag.IsLoggedIn)
        {
            ViewBag.UserRole = JwtHelper.GetClaimValue(token ?? "", ClaimTypes.Role);
            ViewBag.UserName = JwtHelper.GetClaimValue(token ?? "", ClaimTypes.Name) ?? "Candidate";
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
