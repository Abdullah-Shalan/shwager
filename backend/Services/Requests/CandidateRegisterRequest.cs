using System.ComponentModel.DataAnnotations;

namespace Final_Project.Services.Requests;

public class CandidateRegisterRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty; 

    public string Password { get; set; } = string.Empty;

    public string ResumeUrl { get; set; } = string.Empty;
}
