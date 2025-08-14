namespace Final_Project.Services.Requests;

public class JobSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<JobTaskSummaryDto>? JobTasks { get; set; }
}