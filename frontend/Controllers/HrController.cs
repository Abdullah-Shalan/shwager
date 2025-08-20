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

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
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
            TempData["SuccessMessage"] = "Job Created Successfully";

        return RedirectToAction(nameof(Jobs));
    }

    [HttpGet]
    public async Task<IActionResult> EditJob(int jobId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var job = await client.GetFromJsonAsync<JobSummaryDto>($"/api/Hr/jobs/{jobId}");
        if (job == null) return NotFound();

        ViewBag.jobId = jobId;
        return View(new JobDto
        {
            Title = job.Title,
            Description = job.Description
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditJob(int jobId, JobDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"/api/Hr/jobs/{jobId}", model);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Job Edited Successfully";

        return RedirectToAction(nameof(Jobs));
    }

    public async Task<IActionResult> ConfirmDeleteJob(int jobId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var job = await client.GetFromJsonAsync<JobSummaryDto>($"/api/Hr/jobs/{jobId}");
        if (job == null) return NotFound();

        return View(job);
    }

    public async Task<IActionResult> DeleteJob(int jobId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"/api/Hr/jobs/{jobId}");

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Job Deleted Successfully";

        return RedirectToAction(nameof(Jobs));
    }


    public async Task<IActionResult> Applicants(int jobId)
    {
        // TODO fetch candidate progress for all candidates in this job
        var client = _clientFactory.CreateClient("ApiClient");
        var candidates = await client.GetFromJsonAsync<List<CandidateProfileDto>>("/api/Hr/candidates");
        var applicants = new List<CandidateProgressDto>();

        if (candidates != null && candidates.Count != 0)
        {
            foreach (var c in candidates)
            {
                if (c.AssignedJobId != null && c.AssignedJobId == jobId)
                {
                    var progress = await client.GetFromJsonAsync<List<CandidateTaskProgressDto>>($"/api/Hr/candidates/{c.Id}/tasks");
                    var applicant = new CandidateProgressDto
                    {
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        TaskProgress = progress
                    };
                    applicants.Add(applicant);
                }
            }
        }

        return View(applicants);
    }

    // ===== Tasks =====
    [HttpGet]
    public IActionResult CreateJobTask(int jobId, string jobTitle)
    {
        ViewBag.jobId = jobId;
        ViewBag.jobTitle = jobTitle;
        return View(new JobTaskDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobTask(int jobId, JobTaskDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.jobId = jobId;
            return View(model);
        }

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync($"/api/Hr/jobs/{jobId}/tasks", model);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Task Created Successfully";

        return RedirectToAction(nameof(Jobs));
    }

    [HttpGet]
    public async Task<IActionResult> EditTask(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var task = await client.GetFromJsonAsync<TaskResponse>($"/api/Hr/tasks/{id}");
        if (task == null) return NotFound();

        return View(new JobTaskDto
        {
            Description = task.Task,
            RequiresFile = task.RequiresFile,
            RequiresVerification = task.RequiresVerification
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(int id, JobTaskDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"/api/Hr/tasks/{id}", model);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Task Edited Successfully";

        return RedirectToAction(nameof(Jobs));
    }

    public async Task<IActionResult> ConfirmDeleteTask(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var task = await client.GetFromJsonAsync<TaskResponse>($"/api/Hr/tasks/{id}");
        if (task == null) return NotFound();

        ViewBag.TaskId = id;
        return View(new JobTaskDto
        {
            Description = task.Task,
            RequiresFile = task.RequiresFile,
            RequiresVerification = task.RequiresVerification
        });

    }

    public async Task<IActionResult> DeleteTask(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"/api/Hr/tasks/{id}");

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Task Deleted Successfully";

        return RedirectToAction(nameof(Jobs));
    }

    // ===== CANDIDATES =====
    public async Task<IActionResult> Candidates()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var candidates = await client.GetFromJsonAsync<List<CandidateProfileDto>>("/api/Hr/candidates");
        return View(candidates ?? []);
    }

    public async Task<IActionResult> CandidateProgress(int candidateId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var candidate = await client.GetFromJsonAsync<CandidateProfileDto>($"/api/Hr/candidates/{candidateId}/profile");
        var progress = await client.GetFromJsonAsync<List<CandidateTaskProgressDto>>($"/api/Hr/candidates/{candidateId}/tasks");

        return View(new CandidateProgressDto
        {
            FirstName = candidate != null ? candidate.FirstName : "",
            LastName = candidate != null ? candidate.LastName : "",
            TaskProgress = progress
        });
    }

    public async Task<IActionResult> DownloadResume(int candidateId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"/api/Hr/candidates/{candidateId}/resume");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Candidate didn't upload their resume.";
            return RedirectToAction(nameof(Candidates));
        }
        var data = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "resume.pdf";

        return File(data, contentType, fileName);
    }



    public async Task<IActionResult> ConfirmDeleteCandidate(int candidateId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var candidate = await client.GetFromJsonAsync<CandidateProfileDto>($"/api/Hr/candidates/{candidateId}/profile");
        if (candidate == null) return NotFound();

        return View(candidate);
    }
    public async Task<IActionResult> DeleteCandidate(int candidateId)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"api/hr/candidates/{candidateId}");
        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Candidate deleted Successfully";

        return RedirectToAction(nameof(Candidates));
    }
}
