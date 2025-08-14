using System.ComponentModel.DataAnnotations;

namespace Final_Project.Entities
{
    public class Job
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public int HrId { get; set; }
        public Hr? Hr { get; set; }

        public ICollection<JobTask>? JobTasks { get; set; }
        public ICollection<Candidate>? Candidates { get; set; }
    }
}