namespace frontend.Models.Requests;


public class CandidateTaskProgressDto
{
    public string Description { get; set; } = string.Empty;

    public Status Status { get; set; }

    public DateTime? CompletedAt { get; set; }
}

