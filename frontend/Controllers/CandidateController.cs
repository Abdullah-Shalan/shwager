using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using frontend.Models.Requests;
using frontend.Models;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace frontend.Controllers;

public class CandidateController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public CandidateController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<List<JobSummaryDto>>("/api/candidate/available-jobs");
        return View(response ?? []);

    }
    public async Task<IActionResult> AssignedJob()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<JobSummaryDto>("/api/candidate/assigned-job");
        return View(response ?? new JobSummaryDto());

    }

    public async Task<IActionResult> Assign(int jobId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsync($"/api/candidate/jobs/{jobId}/assign", null);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(AssignedJob));
        }
        TempData["ErrorMessage"] = "Unable to assign to this job. Please try again.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> TaskProgress()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<List<CandidateTaskDto>>($"/api/candidate/tasks");

        var job = await client.GetFromJsonAsync<JobSummaryDto>("/api/candidate/assigned-job");
        ViewBag.JobTitle = job?.Title;
        return View(response ?? []);
    }

    public async Task<IActionResult> ViewProfile()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<CandidateProfileDto>($"/api/candidate/view-profile");

        var job = await client.GetFromJsonAsync<JobSummaryDto>("/api/candidate/assigned-job");
        if (response!= null) response.AssignedJobTitle = job?.Title ?? "";
        return View(response);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<CandidateProfileDto>($"/api/candidate/view-profile");

        return View(new CandidateDto
        {
            FirstName = response?.FirstName ?? "",
            LastName = response?.LastName ?? ""
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(CandidateDto candidate)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"/api/candidate/profile", candidate);

        return RedirectToAction(nameof(ViewProfile));
    }
}
