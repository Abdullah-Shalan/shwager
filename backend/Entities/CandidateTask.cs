
namespace Final_Project.Entities
{
    public class CandidateTask
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }
        public Candidate? Candidate { get; set; }

        public int JobTaskId { get; set; }
        public JobTask? JobTask { get; set; }

        public Status Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public bool IsVerifiedByHr { get; set; } = false;

    }

}