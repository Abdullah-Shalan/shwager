namespace frontend.Models.Requests;


public class JobTaskDto
{
    public string Description { get; set; } = string.Empty;
    public bool RequiresFile { get; set; } = false;
    public bool RequiresVerification { get; set; } = false;

}