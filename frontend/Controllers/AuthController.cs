using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using frontend.Models.Requests;
using frontend.Models;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace frontend.Controllers;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AuthController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(CandidateRegisterRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _clientFactory.CreateClient("ApiClient");

        var response = await client.PostAsJsonAsync("/api/auth/candidate/register", model);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login", "Auth");
        }

        ModelState.AddModelError(string.Empty, "Invalid attempt.");
        return View(model);
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _clientFactory.CreateClient("ApiClient");

        var response = await client.PostAsJsonAsync("/api/auth/login", model);

        if (response.IsSuccessStatusCode)
        {
            var Token = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(Token))
            {
                // Save JWT in session
                HttpContext.Session.SetString("JWToken", Token);

                if (JwtHelper.GetClaimValue(Token, ClaimTypes.Role) == "Hr")
                    return RedirectToAction("Index", "Hr");

                return RedirectToAction("Index", "Candidate");
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JWToken");
        return RedirectToAction("Login");
    }
}
