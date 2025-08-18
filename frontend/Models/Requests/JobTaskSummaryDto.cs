namespace frontend.Models.Requests;


public class JobTaskSummaryDto
{
    public int Id { get; set; }
    public int TaskOrder { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool RequiresFile { get; set; } = false;
    public bool RequiresVerification { get; set; } = false;
}