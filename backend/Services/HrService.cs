using Final_Project.DAL.Repositories;
using Final_Project.Entities;
using Final_Project.Services.Interfaces;
using Final_Project.Services.Requests;

namespace Final_Project.Services;

public class HrService : IHrService
{
    private readonly UnitOfWork _unitOfWork;

    public HrService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Job> CreateJobAsync(int hrId, JobDto jobDto)
    {
        Job job = new Job
        {
            Title = jobDto.Title,
            Description = jobDto.Description,
            HrId = hrId
        };

        await _unitOfWork.Jobs.InsertAsync(job);
        await _unitOfWork.SaveAsync();

        return job;
    }

    public async Task<JobTask?> CreateJobTaskAsync(int jobId, JobTaskDto jobTaskDto)
    {
        var job = await _unitOfWork.Jobs.GetFirstOrDefaultAsync(
            j => j.Id == jobId, includeProperties: "JobTasks");
        if (job == null)
            return null;

        int nextOrder = 1;
        if (job.JobTasks != null && job.JobTasks.Count != 0)
            nextOrder = job.JobTasks.Max(t => t.Order) + 1;

        var jobTask = new JobTask()
        {
            Description = jobTaskDto.Description,
            RequiresFile = jobTaskDto.RequiresFile,
            RequiresVerification = jobTaskDto.RequiresVerification,
            Order = nextOrder,
            JobId = jobId,
            Job = job
        };

        await _unitOfWork.JobTasks.InsertAsync(jobTask);
        await _unitOfWork.SaveAsync();

        return jobTask;

    }

    public async Task<bool> DeleteCandidateAsync(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(c => c.Id == candidateId);
        if (candidate == null)
            return false;

        await _unitOfWork.Candidates.DeleteAsync(candidate);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> DeleteJobAsync(int jobId)
    {
        var job = await _unitOfWork.Jobs.GetFirstOrDefaultAsync(
            filter: j => j.Id == jobId,
            includeProperties: "JobTasks,Candidates");
        if (job == null)
            return false;

        if (job.JobTasks != null)
        {
            foreach (var task in job.JobTasks.ToList())
            {
                var cTasks = await _unitOfWork.CandidateTasks.GetAsync(ct => ct.JobTaskId == task.Id);
                foreach (var ct in cTasks)
                {
                    if (ct != null) await _unitOfWork.CandidateTasks.DeleteAsync(ct);
                }
                await _unitOfWork.JobTasks.DeleteAsync(task);
            }
        }
        if (job.Candidates != null)
        {
            foreach (var candidate in job.Candidates)
            {
                candidate.Job = null;
                await _unitOfWork.Candidates.UpdateAsync(candidate);
            }
        }

        await _unitOfWork.Jobs.DeleteAsync(job);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> DeleteJobTaskAsync(int jobTaskId)
    {
        var jobTask = await _unitOfWork.JobTasks.GetFirstOrDefaultAsync(j => j.Id == jobTaskId);
        if (jobTask == null)
            return false;

        await _unitOfWork.JobTasks.DeleteAsync(jobTask);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<ResumeFileDto?> DownloadResume(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(c => c.Id == candidateId);
        if (candidate?.ResumeData == null)
            return null;

        return new ResumeFileDto
        {
            Data = candidate.ResumeData,
            FileName = candidate.ResumeFileName ?? "resume.pdf",
            ContentType = "application/octet-stream"
        };
    }

    public async Task<bool> EditJobAsync(int jobId, JobDto jobDto)
    {
        var job = await _unitOfWork.Jobs.GetFirstOrDefaultAsync(j => j.Id == jobId);
        if (job == null)
            return false;

        job.Title = jobDto.Title;
        job.Description = jobDto.Description;

        await _unitOfWork.Jobs.UpdateAsync(job);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> EditJobTaskAsync(int jobTaskId, JobTaskDto jobTaskDto)
    {
        var jobTask = await _unitOfWork.JobTasks.GetFirstOrDefaultAsync(j => j.Id == jobTaskId);
        if (jobTask == null)
            return false;

        jobTask.Description = jobTaskDto.Description;
        jobTask.RequiresFile = jobTaskDto.RequiresFile;
        jobTask.RequiresVerification = jobTaskDto.RequiresVerification;

        await _unitOfWork.JobTasks.UpdateAsync(jobTask);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<IEnumerable<CandidateProfileDto>?> GetAllCandidatesAsync()
    {
        var candidates = await _unitOfWork.Candidates.GetAsync(
            includeProperties: "Job"
        );
        var response = new List<CandidateProfileDto>();

        if (candidates != null)
        {
            foreach (var candidate in candidates.ToList())
            {
                response.Add(new CandidateProfileDto
                {
                    Id = candidate.Id,
                    Email = candidate.Email,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    AssignedJobId = candidate.JobId,
                    AssignedJobTitle = candidate.Job?.Title ?? "None"
                });
            }
        }

        return response;

    }

    public async Task<IEnumerable<JobSummaryDto>?> GetAvailableJobsAsync()
    {
        var jobs = await _unitOfWork.Jobs.GetAsync(includeProperties: "JobTasks");
        if (jobs == null)
            return null;

        var response = jobs.Select(job => new JobSummaryDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description ?? "",
            JobTasks = job.JobTasks?.Select(task => new JobTaskSummaryDto
            {
                Id = task.Id,
                TaskOrder = task.Order,
                Description = task.Description,
                RequiresFile = task.RequiresFile,
                RequiresVerification = task.RequiresVerification
            }).ToList()
        }).ToList();

        return response;
    }

    public async Task<CandidateProfileDto?> GetCandidateProfileAsync(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(
                filter: c => c.Id == candidateId,
                includeProperties: "Job"
        );
        if (candidate == null)
            return null;

        string AssignedJobTitle = candidate.Job?.Title ?? "None";

        var candidateProfile = new CandidateProfileDto
        {
            Id = candidate.Id,
            Email = candidate.Email,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            AssignedJobId = candidate.JobId,
            AssignedJobTitle = AssignedJobTitle
        };

        return candidateProfile;
    }

    public async Task<IEnumerable<CandidateTaskDto>?> GetCandidateTaskProgressAsync(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(
                filter: c => c.Id == candidateId,
                includeProperties: "CandidateTasks.JobTask"
        );

        if (candidate == null || candidate.CandidateTasks == null)
            return null;

        List<CandidateTaskDto> response = candidate.CandidateTasks
            .OrderBy(t => t.JobTask?.Order ?? int.MaxValue)
            .Select(task => new CandidateTaskDto
            {
                Id = task.Id,
                Order = task.JobTask?.Order ?? 0,
                Description = task.JobTask?.Description ?? "",
                RequiresVerification = task.JobTask?.RequiresVerification ?? false,
                IsVerifiedByHr = task.IsVerifiedByHr,
                RequiresFile = task.JobTask?.RequiresFile ?? false,
                FilePath = task.FilePath,
                Status = task.Status,
                CompletedAt = task.CompletedAt
            }).ToList();

        return response;

    }

    public async Task<JobSummaryDto?> GetJobByIdAsync(int jobId)
    {
        var job = await _unitOfWork.Jobs.GetFirstOrDefaultAsync(
            j => j.Id == jobId,
            includeProperties: "JobTasks"
        );

        if (job == null)
            return null;

        return new JobSummaryDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description ?? "",
            JobTasks = job.JobTasks?.Select(task => new JobTaskSummaryDto
            {
                Id = task.Id,
                TaskOrder = task.Order,
                Description = task.Description,
                RequiresFile = task.RequiresFile,
                RequiresVerification = task.RequiresVerification
            }).ToList()
        };
    }

    public async Task<JobTask?> GetJobTaskById(int jobTaskId)
    {
        var task = await _unitOfWork.JobTasks.GetFirstOrDefaultAsync(
           t => t.Id == jobTaskId,
           includeProperties: "Job");

        if (task == null)
            return null;

        return task;
    }

    public async Task<bool> ReorderJobTasksAsync(int jobId, List<ReorderRequest> newOrders)
    {
        var tasks = await _unitOfWork.JobTasks.GetAsync(t => t.JobId == jobId);

        foreach (var newOrder in newOrders)
        {
            var task = tasks.FirstOrDefault(t => t.Id == newOrder.TaskId);
            if (task != null)
            {
                task.Order = newOrder.Order;
                await _unitOfWork.JobTasks.UpdateAsync(task);
            }
        }

        await _unitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> SetTaskFileRequirementAsync(int jobTaskId)
    {
        var task = await _unitOfWork.JobTasks
                    .GetFirstOrDefaultAsync(t => t.Id == jobTaskId);
        if (task == null)
            return false;

        task.RequiresFile = true;

        await _unitOfWork.JobTasks.UpdateAsync(task);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> SetTaskVerificationRequirementAsync(int jobTaskId)
    {
        var task = await _unitOfWork.JobTasks
                    .GetFirstOrDefaultAsync(t => t.Id == jobTaskId);
        if (task == null)
            return false;

        task.RequiresVerification = true;

        await _unitOfWork.JobTasks.UpdateAsync(task);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> VerifyCompletedTaskAsync(int candidateTaskId)
    {
        var task = await _unitOfWork.CandidateTasks
                    .GetFirstOrDefaultAsync(t => t.Id == candidateTaskId);

        if (task == null)
            return false;

        task.IsVerifiedByHr = true;

        await _unitOfWork.CandidateTasks.UpdateAsync(task);
        await _unitOfWork.SaveAsync();

        return true;
    }
}