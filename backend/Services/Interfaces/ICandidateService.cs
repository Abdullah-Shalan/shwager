using Final_Project.Entities;
using Final_Project.Services.Requests;

namespace Final_Project.Services.Interfaces;

public interface ICandidateService
{
    // Job Interaction
    Task<IEnumerable<JobSummaryDto>?> GetAvailableJobsAsync();
    Task<JobSummaryDto?> GetAssignedJob(int candidateId);
    Task<bool> AssignToJobAsync(int candidateId, int jobId);

    // Task Interaction
    Task<IEnumerable<CandidateTaskDto>?> GetAssignedTasksAsync(int candidateId);
    Task<DateTime?> GetCompletionTimestampAsync(int candidateTaskId);
    Task<bool> CompleteTaskAsync(int candidateTaskId);

    // Profile
    Task<CandidateProfileDto?> GetCandidateProfile(int candidateId);
    Task<bool> EditProfileInfoAsync(int candidateId, CandidateDto candidateDto);
    Task<bool> UploadResume(int candidateId, IFormFile resume);
    Task<ResumeFileDto?> DownloadResume(int candidateId);

}