using Final_Project.DAL.Repositories;
using Final_Project.Entities;
using Final_Project.Services.Interfaces;
using Final_Project.Services.Requests;

namespace Final_Project.Services;

public class CandidateService : ICandidateService
{
    private readonly UnitOfWork _unitOfWork;

    public CandidateService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AssignToJobAsync(int candidateId, int jobId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(c => c.Id == candidateId);
        var job = await _unitOfWork.Jobs.GetFirstOrDefaultAsync(
            j => j.Id == jobId,
            includeProperties: "JobTasks");
        if (candidate == null || job == null) return false;

        candidate.JobId = jobId;
        await _unitOfWork.Candidates.UpdateAsync(candidate);

        // Remove old tasks
        var oldTasks = await _unitOfWork.CandidateTasks.GetAsync(ct => ct.CandidateId == candidateId);
        if (oldTasks.Any())
        {
            foreach (var ot in oldTasks)
            {
                await _unitOfWork.CandidateTasks.DeleteAsync(ot);
            }
        }

        if (job.JobTasks != null)
        {
            foreach (var task in job.JobTasks)
            {
                var candidateTask = new CandidateTask
                {
                    CandidateId = candidateId,
                    JobTaskId = task.Id,
                    Status = Status.NotStarted
                };
                await _unitOfWork.CandidateTasks.InsertAsync(candidateTask);
            }
        }

        await _unitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> CompleteTaskAsync(int candidateTaskId)
    {
        var cTask = await _unitOfWork.CandidateTasks
                        .GetFirstOrDefaultAsync(ct =>
                            ct.Id == candidateTaskId);

        if (cTask == null) return false;

        cTask.Status = Status.Completed;
        cTask.CompletedAt = DateTime.Now;
        StartNextTask(cTask);

        await _unitOfWork.CandidateTasks.UpdateAsync(cTask);
        await _unitOfWork.SaveAsync();

        return true;
    }
    private async void StartNextTask(CandidateTask cTask)
    {
        var cTasks = cTask.Candidate?.CandidateTasks;
        if (cTasks == null) return;

        var nextTask = cTasks
            .Where(ct => ct.JobTask?.Order > cTask.JobTask?.Order)
            .OrderBy(ct => ct.JobTask?.Order)
            .FirstOrDefault();

        if (nextTask == null) return;

        nextTask.Status = Status.InProgress;

        await _unitOfWork.CandidateTasks.UpdateAsync(nextTask);
        await _unitOfWork.SaveAsync();
    }

    public async Task<bool> EditProfileInfoAsync(int candidateId, CandidateDto candidateDto)
    {
        var candidate = await _unitOfWork.Candidates
                        .GetFirstOrDefaultAsync(c => c.Id == candidateId);
        if (candidate == null) return false;

        candidate.FirstName = candidateDto.FirstName;
        candidate.LastName = candidateDto.LastName;


        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<IEnumerable<CandidateTaskDto>?> GetAssignedTasksAsync(int candidateId)
    {
        var cTasks = await _unitOfWork.CandidateTasks.GetAsync(
                filter: ct => ct.CandidateId == candidateId,
                includeProperties: "JobTask");

        if (cTasks == null)
            return null;

        List<CandidateTaskDto> response = cTasks
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
            JobTasks = job.JobTasks?
                .OrderBy(t => t.Order)
                .Select(task => new JobTaskSummaryDto
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

    public async Task<JobSummaryDto?> GetAssignedJob(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(
            filter: c => c.Id == candidateId,
            includeProperties: "Job,Job.JobTasks");

        if (candidate == null || candidate.Job == null)
            return null;

        var response = new JobSummaryDto
        {
            Id = candidate.Job.Id,
            Title = candidate.Job.Title,
            Description = candidate.Job.Description ?? "",
            JobTasks = candidate.Job.JobTasks?
                .OrderBy(t => t.Order)
                .Select(task => new JobTaskSummaryDto
                {
                    Id = task.Id,
                    TaskOrder = task.Order,
                    Description = task.Description,
                    RequiresFile = task.RequiresFile,
                    RequiresVerification = task.RequiresVerification
                }).ToList()
        };

        return response;
    }

    public async Task<DateTime?> GetCompletionTimestampAsync(int candidateTaskId)
    {
        var cTask = await _unitOfWork.CandidateTasks
                        .GetFirstOrDefaultAsync(ct => ct.Id == candidateTaskId);
        if (cTask == null) return null;
        return cTask.CompletedAt;
    }

    public async Task<bool> UploadResume(int candidateId, IFormFile resume)
    {
        if (resume == null || resume.Length == 0)
            return false;

        // Define storage folder
        var folderPath = Path.Combine("Uploads", "Resumes", candidateId.ToString());

        // Ensure folder exists
        Directory.CreateDirectory(folderPath);

        // Create unique file name to avoid overwriting
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(resume.FileName)}";
        var filePath = Path.Combine(folderPath, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await resume.CopyToAsync(stream);
        }

        // Store relative path or full URL in DB
        var resumeUrl = Path.Combine("Resumes", candidateId.ToString(), uniqueFileName)
                            .Replace("\\", "/"); // make web-friendly

        // Update candidate
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(
                        c => c.Id == candidateId);
        if (candidate != null)
        {
            candidate.ResumeUrl = resumeUrl;
            await _unitOfWork.Candidates.UpdateAsync(candidate);
            await _unitOfWork.SaveAsync();
        }

        return true;
    }

    public async Task<CandidateProfileDto?> GetCandidateProfile(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(
                            c => c.Id == candidateId);
        if (candidate == null) return null;

        return new CandidateProfileDto
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            ResumeUrl = candidate.ResumeUrl
        };
    }
}