using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_Project.Services.Interfaces;
using Final_Project.Services.Requests;
using System.Security.Claims;


namespace Final_Project.API.Controllers;

[ApiController]
[Authorize(Roles = "Hr")]
[Route("api/[controller]")]
public class HrController : ControllerBase
{
    private readonly IHrService _service;

    public HrController(IHrService service)
    {
        _service = service;
    }

    // ===== Job Management =====

    [AllowAnonymous]
    [HttpGet("jobs")]
    public async Task<ActionResult<IEnumerable<JobSummaryDto>>> GetAvailableJobs()
    {
        var jobs = await _service.GetAvailableJobsAsync();
        if (jobs == null) return NotFound();
        return Ok(jobs);
    }

    [HttpGet("jobs/{jobId}")]
    public async Task<ActionResult<JobSummaryDto>> GetJobById(int jobId)
    {
        var job = await _service.GetJobByIdAsync(jobId);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost("jobs")]
    public async Task<ActionResult<JobResponse>> CreateJob([FromBody] JobDto jobDto)
    {
        int hrId = GetHrIdFromClaims();
        var createdJob = await _service.CreateJobAsync(hrId, jobDto);
        var response = new JobResponse
        {
            Id = createdJob.Id,
            Title = createdJob.Title,
            Description = createdJob.Description ?? "",
            AssignedHrName = GetHrNameFromClaims()
        };
        return CreatedAtAction(nameof(GetJobById), new { jobId = createdJob.Id }, response);
    }

    [HttpPut("jobs/{jobId}")]
    public async Task<IActionResult> EditJob(int jobId, [FromBody] JobDto jobDto)
    {
        var success = await _service.EditJobAsync(jobId, jobDto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("jobs/{jobId}")]
    public async Task<IActionResult> DeleteJob(int jobId)
    {
        var success = await _service.DeleteJobAsync(jobId);
        if (!success) return NotFound();
        return NoContent();
    }

    // ===== Job Task Management =====

    [HttpPost("jobs/{jobId}/tasks")]
    public async Task<ActionResult<TaskResponse>> CreateJobTask(int jobId, [FromBody] JobTaskDto jobTaskDto)
    {
        var createdTask = await _service.CreateJobTaskAsync(jobId, jobTaskDto);
        if (createdTask == null || createdTask.Job == null)
            return NotFound($"Job with id {jobId} was not found");

        var response = new TaskResponse
        {
            Id = createdTask.Id,
            ForJobTitle = createdTask.Job.Title,
            Task = createdTask.Description,
            Order = createdTask.Order,
            RequiresFile = createdTask.RequiresFile,
            RequiresVerification = createdTask.RequiresVerification,
            AssignedHrName = GetHrNameFromClaims()
        };

        return CreatedAtAction(nameof(GetJobTaskById), new { jobTaskId = createdTask.Id }, response);
    }

    [HttpGet("tasks/{jobTaskId}")]
    public async Task<ActionResult<TaskResponse>> GetJobTaskById(int jobTaskId)
    {
        var jobTask = await _service.GetJobTaskById(jobTaskId);
        if (jobTask == null || jobTask.Job == null)
            return NotFound();

        var response = new TaskResponse
        {
            Id = jobTask.Id,
            ForJobTitle = jobTask.Job.Title,
            Task = jobTask.Description,
            RequiresFile = jobTask.RequiresFile,
            RequiresVerification = jobTask.RequiresVerification,
            AssignedHrName = GetHrNameFromClaims()
        };

        return Ok(response);
    }

    [HttpPut("tasks/{jobTaskId}")]
    public async Task<IActionResult> EditJobTask(int jobTaskId, [FromBody] JobTaskDto jobTaskDto)
    {
        var success = await _service.EditJobTaskAsync(jobTaskId, jobTaskDto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("tasks/{jobTaskId}")]
    public async Task<IActionResult> DeleteJobTask(int jobTaskId)
    {
        var success = await _service.DeleteJobTaskAsync(jobTaskId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("jobs/{jobId}/tasks/reorder")]
    public async Task<IActionResult> ReorderJobTasks(int jobId, [FromBody] List<ReorderRequest> newOrders)
    {
        var success = await _service.ReorderJobTasksAsync(jobId, newOrders);
        if (!success) return BadRequest("Failed to reorder tasks.");
        return NoContent();
    }

    [HttpPut("tasks/{jobTaskId}/set-file-requirement")]
    public async Task<IActionResult> SetTaskFileRequirement(int jobTaskId)
    {
        var success = await _service.SetTaskFileRequirementAsync(jobTaskId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("tasks/{jobTaskId}/set-verification-requirement")]
    public async Task<IActionResult> SetTaskVerificationRequirement(int jobTaskId)
    {
        var success = await _service.SetTaskVerificationRequirementAsync(jobTaskId);
        if (!success) return NotFound();
        return NoContent();
    }

    // ===== Candidate Management =====
    [HttpGet("candidates")]
    public async Task<ActionResult<IEnumerable<CandidateProfileDto>>> GetAllCandidates()
    {
        var response = await _service.GetAllCandidatesAsync();
        return Ok(response);
    }

    [HttpGet("candidates/{candidateId}/profile")]
    public async Task<ActionResult<CandidateProfileDto>> GetCandidateProfile(int candidateId)
    {
        var profile = await _service.GetCandidateProfileAsync(candidateId);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpGet("candidates/{candidateId}/resume")]
    public async Task<ActionResult> DownloadResume(int candidateId)
    {
        var resume = await _service.DownloadResume(candidateId);
        if (resume == null) return NotFound();
        return File(resume.Data, resume.ContentType, resume.FileName);
    }

    [HttpGet("candidates/{candidateId}/tasks")]
    public async Task<ActionResult<IEnumerable<CandidateTaskDto>>> GetCandidateTaskProgress(int candidateId)
    {
        var tasks = await _service.GetCandidateTaskProgressAsync(candidateId);
        if (tasks == null) return NotFound();
        return Ok(tasks);
    }

    [HttpPut("candidates/tasks/{candidateTaskId}/verify")]
    public async Task<IActionResult> VerifyCompletedTask(int candidateTaskId)
    {
        var success = await _service.VerifyCompletedTaskAsync(candidateTaskId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("candidates/{candidateId}")]
    public async Task<IActionResult> DeleteCandidate(int candidateId)
    {
        var success = await _service.DeleteCandidateAsync(candidateId);
        if (!success) return NotFound();
        return NoContent();
    }

    private int GetHrIdFromClaims()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out int hrId))
        {
            return hrId;
        }
        throw new UnauthorizedAccessException("HR Id not found in token.");
    }
    private string GetHrNameFromClaims()
    {
        var claim = User.FindFirst(ClaimTypes.Name);
        if (claim != null)
        {
            return claim.Value;
        }
        throw new UnauthorizedAccessException("HR name not found in token.");
    }

}

