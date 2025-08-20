namespace frontend.Models.Requests;

public class CandidateProgressDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public List<CandidateTaskProgressDto>? TaskProgress { get; set; }

}