namespace Final_Project.Entities
{
    public class JobTask
    {
        public int Id { get; set; }

        public required string Description { get; set; }

        public bool RequiresFile{ get; set; } = false;

        public bool RequiresVerification { get; set; } = false;

        public int Order { get; set; }
        
        public int JobId { get; set; }
        public Job? Job { get; set; }

        public ICollection<CandidateTask>? CandidateTasks { get; set; }
    }

}