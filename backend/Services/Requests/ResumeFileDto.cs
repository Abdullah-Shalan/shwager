namespace Final_Project.Services.Requests;

public class ResumeFileDto
{
    public byte[] Data { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; } = "application/octet-stream";
}