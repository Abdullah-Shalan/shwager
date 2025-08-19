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

    public async Task<IActionResult> CompleteTask(int taskId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsync($"/api/candidate/tasks/{taskId}/complete", null);

        TempData["CompleteTaskMessage"] = response.IsSuccessStatusCode
        ? "Task completed successfully."
        : "Failed to complete task.";

        return RedirectToAction(nameof(TaskProgress));
    }


    public async Task<IActionResult> ViewProfile()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<CandidateProfileDto>($"/api/candidate/view-profile");

        var job = await client.GetFromJsonAsync<JobSummaryDto>("/api/candidate/assigned-job");
        if (response != null) response.AssignedJobTitle = job?.Title ?? "";
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

    [HttpPost]
    public async Task<IActionResult> UploadResume(IFormFile resume)
    {
        if (resume == null || resume.Length == 0)
        {
            TempData["UploadMessage"] = "No file selected.";
            return RedirectToAction("ViewProfile");
        }

        var client = _clientFactory.CreateClient("ApiClient");

        using var content = new MultipartFormDataContent
        {
            { new StreamContent(resume.OpenReadStream()), "resume", resume.FileName }
        };

        var response = await client.PostAsync("/api/candidate/upload-resume", content);

        TempData["UploadMessage"] = response.IsSuccessStatusCode
            ? "Resume uploaded successfully."
            : "Failed to upload resume.";

        return RedirectToAction("ViewProfile");
    }

    public async Task<IActionResult> DownloadResume()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"/api/candidate/resume");

        if (!response.IsSuccessStatusCode)
            return NotFound("Resume not found.");

        var data = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "resume.pdf";

        return File(data, contentType, fileName);
    }
}
