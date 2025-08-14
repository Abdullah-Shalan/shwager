using Final_Project.Entities;
namespace Final_Project.Services.Requests;

public class CandidateTaskProgressDto
{
    public string Description { get; set; } = string.Empty;

    public Status Status { get; set; }

    public string DocumentUrl { get; set; } = string.Empty;

    public DateTime? CompletedAt { get; set; }
}

