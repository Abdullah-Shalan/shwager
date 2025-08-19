using Final_Project.Entities;
using Final_Project.Services.Requests;

namespace Final_Project.Services.Interfaces;

public interface IHrService
{
    // Job Management 
    Task<IEnumerable<JobSummaryDto>?> GetAvailableJobsAsync();
    Task<JobSummaryDto?> GetJobByIdAsync(int jobId);
    Task<Job> CreateJobAsync(int hrId, JobDto jobDto);
    Task<bool> EditJobAsync(int jobId, JobDto jobDto);
    Task<bool> DeleteJobAsync(int jobId);


    // Job Task Management 
    Task<JobTask?> CreateJobTaskAsync(int jobId, JobTaskDto jobTaskDto);
    Task<JobTask?> GetJobTaskById(int jobTaskId);
    Task<bool> EditJobTaskAsync(int jobTaskId, JobTaskDto jobTaskDto);
    Task<bool> DeleteJobTaskAsync(int jobTaskId);
    Task<bool> ReorderJobTasksAsync(int jobId, List<ReorderRequest> newOrders);
    Task<bool> SetTaskFileRequirementAsync(int jobTaskId);
    Task<bool> SetTaskVerificationRequirementAsync(int jobTaskId);


    // Candidate Management
    Task<IEnumerable<CandidateProfileDto>?> GetAllCandidatesAsync();
    Task<ResumeFileDto?> DownloadResume(int candidateId);
    Task<CandidateProfileDto?> GetCandidateProfileAsync(int candidateId);
    Task<IEnumerable<CandidateTaskDto>?> GetCandidateTaskProgressAsync(int candidateId);
    Task<bool> VerifyCompletedTaskAsync(int candidateTaskId);
    Task<bool> DeleteCandidateAsync(int candidateId);
}