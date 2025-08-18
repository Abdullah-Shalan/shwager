using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using frontend.Models.Requests;
using frontend.Models;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace frontend.Controllers;

public class HrController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public HrController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    // ===== JOBS =====
    public async Task<IActionResult> Jobs()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetFromJsonAsync<List<JobSummaryDto>>("/api/Hr/jobs");
        return View(response ?? []);
    }

    public async Task<IActionResult> JobDetails(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"/api/Hr/jobs/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var job = await response.Content.ReadFromJsonAsync<JobSummaryDto>();
        return View(job);
    }

    [HttpGet]
    public IActionResult CreateJob() => View();

    [HttpPost]
    public async Task<IActionResult> CreateJob(JobDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("/api/Hr/jobs", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Jobs));

        ModelState.AddModelError(string.Empty, "Failed to create job.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditJob(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var job = await client.GetFromJsonAsync<JobSummaryDto>($"/api/Hr/jobs/{id}");
        if (job == null) return NotFound();

        return View(new JobDto
        {
            Title = job.Title,
            Description = job.Description
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditJob(int id, JobDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"/api/Hr/jobs/{id}", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Jobs));

        ModelState.AddModelError(string.Empty, "Failed to update job.");
        return View(model);
    }

    public async Task<IActionResult> DeleteJob(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        await client.DeleteAsync($"/jobs/{id}");
        return RedirectToAction(nameof(Jobs));
    }

    // ===== Tasks =====
    [HttpGet]
    public IActionResult CreateJobTask(int jobId)
    {
        ViewBag.jobId = jobId;
        return View(new JobTaskDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobTask(int jobId, [FromBody] JobTaskDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.jobId = jobId;
            return View(model);
        }

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync($"/api/Hr/jobs/{jobId}/tasks", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("JobDetails", new { id = jobId });

        ModelState.AddModelError(string.Empty, "Failed to create job.");
        ViewBag.jobId = jobId;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditJobTask(int id, int jobId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var task = await client.GetFromJsonAsync <TaskResponse>($"/api/Hr/tasks/{id}");
        if (task == null) return NotFound();

        ViewBag.jobId = jobId;
        return View(new JobTaskDto
        {
            Description = task.Description,
            RequiresFile = task.RequiresFile,
            RequiresVerification = task.RequiresVerification
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditJobTask(int id, int jobId, JobDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"/api/Hr/tasks/{id}", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("JobDetails", new { id = jobId });

        ModelState.AddModelError(string.Empty, "Failed to update job.");
        return View(model);
    }

    public async Task<IActionResult> DeleteJobTask(int id, int jobId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        await client.DeleteAsync($"/api/Hr/tasks/{id}");
        return RedirectToAction("JobDetails", new { id = jobId });
    }

    // ===== CANDIDATES =====
    public async Task<IActionResult> Candidates()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var candidates = await client.GetFromJsonAsync<List<CandidateProfileDto>>("/api/Hr/candidates");
        return View(candidates ?? new List<CandidateProfileDto>());
    }

    public async Task<IActionResult> CandidateProfile(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var profile = await client.GetFromJsonAsync<CandidateProfileDto>($"/api/Hr/candidates/{id}/profile");
        if (profile == null) return NotFound();

        var tasks = await client.GetFromJsonAsync<List<CandidateTaskDto>>($"/api/Hr/candidates/{id}/tasks");
        ViewBag.Tasks = tasks ?? new List<CandidateTaskDto>();

        return View(profile);
    }

    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        await client.DeleteAsync($"/candidates/{id}");
        return RedirectToAction(nameof(Candidates));
    }
}
