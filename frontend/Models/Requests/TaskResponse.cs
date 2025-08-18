namespace frontend.Models.Requests
{
    public class TaskResponse
    {
        public int Id { get; set; }
        
        public string ForJobTitle { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool RequiresFile{ get; set; } = false;

        public bool RequiresVerification { get; set; } = false;

        public int Order { get; set; }

        public string AssignedHrName { get; set; } = string.Empty;
    }
}