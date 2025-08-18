namespace frontend.Models.Requests;


public class CandidateProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty; 

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string ResumeUrl { get; set; } = string.Empty;

    public string AssignedJobTitle { get; set; } = string.Empty;

}