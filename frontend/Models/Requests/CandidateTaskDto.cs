
namespace frontend.Models.Requests;


public class CandidateTaskDto
{
    public int Id { get; set; }
    public int Order { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool RequiresVerification { get; set; } = false;
    public bool IsVerifiedByHr { get; set; } = false;
    public bool RequiresFile { get; set; } = false;
    public string FilePath { get; set; } = string.Empty;

    public Status Status { get; set; }
    public DateTime? CompletedAt { get; set; }
}