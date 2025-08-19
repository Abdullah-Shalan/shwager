using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_Project.Services.Interfaces;
using Final_Project.Services.Requests;
using System.Security.Claims;

namespace Final_Project.API.Controllers;

[ApiController]
[Authorize(Roles = "Candidate")]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly ICandidateService _service;

    public CandidateController(ICandidateService service)
    {
        _service = service;
    }

    // ===== Job Interaction =====
    [HttpGet("available-jobs")]
    public async Task<ActionResult<IEnumerable<JobSummaryDto>>> GetAvailableJobs()
    {
        var jobs = await _service.GetAvailableJobsAsync();
        if (jobs == null) return NotFound();
        return Ok(jobs);
    }

    [HttpGet("assigned-job")]
    public async Task<ActionResult<JobSummaryDto>> GetAssignedJob()
    {
        var candidateId = GetCandidateIdFromClaims();
        var jobs = await _service.GetAssignedJob(candidateId);
        if (jobs == null) return NotFound();
        return Ok(jobs);
    }

    [HttpPost("jobs/{jobId}/assign")]
    public async Task<IActionResult> AssignToJob(int jobId)
    {
        var candidateId = GetCandidateIdFromClaims();
        var result = await _service.AssignToJobAsync(candidateId, jobId);
        return result ? Ok() : BadRequest("Unable to assign candidate to job.");
    }

    // ===== Task Interaction =====
    [HttpGet("tasks")]
    public async Task<ActionResult<IEnumerable<CandidateTaskDto>>> GetAssignedTasks()
    {
        var candidateId = GetCandidateIdFromClaims();
        var tasks = await _service.GetAssignedTasksAsync(candidateId);
        return Ok(tasks);
    }

    [HttpGet("tasks/{candidateTaskId}/completion-timestamp")]
    public async Task<ActionResult<DateTime?>> GetCompletionTimestamp(int candidateTaskId)
    {
        var timestamp = await _service.GetCompletionTimestampAsync(candidateTaskId);
        if (timestamp == null)
            return Ok($"Candidate Task {candidateTaskId} is not completed");
        return Ok(timestamp);
    }

    [HttpPost("tasks/{candidateTaskId}/complete")]
    public async Task<IActionResult> CompleteTask(int candidateTaskId)
    {
        var result = await _service.CompleteTaskAsync(candidateTaskId);
        return result ? Ok() : BadRequest("Unable to complete task.");
    }

    // ===== Profile =====
    [HttpGet("view-profile")]
    public async Task<ActionResult<CandidateProfileDto>> ViewProfile()
    {
        var candidateId = GetCandidateIdFromClaims();
        var result = await _service.GetCandidateProfile(candidateId);
        if (result == null) return BadRequest("Unable to find candidate profile.");
        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> EditProfileInfo([FromBody] CandidateDto candidateDto)
    {
        var candidateId = GetCandidateIdFromClaims();
        var result = await _service.EditProfileInfoAsync(candidateId, candidateDto);
        return result ? Ok() : BadRequest("Unable to update profile.");
    }

    [HttpPost("upload-resume")]
    public async Task<IActionResult> UploadResume(IFormFile resume)
    {
        var candidateId = GetCandidateIdFromClaims();
        var result = await _service.UploadResume(candidateId, resume);
        return result ? Ok("Resume uploaded successfully") : BadRequest("Resume upload failed.");
    }

    [HttpGet("resume")]
    public async Task<ActionResult> DownloadResume()
    {
        var candidateId = GetCandidateIdFromClaims();
        var resume = await _service.DownloadResume(candidateId);
        if (resume == null) return NotFound();
        return File(resume.Data, resume.ContentType, resume.FileName);
    }

    private int GetCandidateIdFromClaims()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out int id))
        {
            return id;
        }
        throw new UnauthorizedAccessException("HR Id not found in token.");
    }


}
