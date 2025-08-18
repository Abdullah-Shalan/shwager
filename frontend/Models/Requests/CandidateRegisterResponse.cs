namespace frontend.Models.Requests;

public class CandidateRegisterResponse
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty; 

    public string ResumeUrl { get; set; } = string.Empty;
}
