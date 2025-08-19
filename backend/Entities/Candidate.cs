using System.ComponentModel.DataAnnotations;

namespace Final_Project.Entities
{
    public class Candidate
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Login Fields

        public string PasswordHash { get; set; } = string.Empty; // Login Fields

        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;

        public byte[]? ResumeData { get; set; }
        public string ResumeFileName { get; set; } = string.Empty;

        public int? JobId { get; set; }
        public Job? Job { get; set; }

        public ICollection<CandidateTask>? CandidateTasks { get; set; }
    }
}