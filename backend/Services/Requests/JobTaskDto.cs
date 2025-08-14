namespace Final_Project.Services.Requests;

public class JobTaskDto
{
    public string Description { get; set; } = string.Empty;
    public bool RequiresFile { get; set; } = false;
    public bool RequiresVerification { get; set; } = false;

}