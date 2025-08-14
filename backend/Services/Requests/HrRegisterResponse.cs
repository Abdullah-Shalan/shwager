using System.ComponentModel.DataAnnotations;

namespace Final_Project.Services.Requests;

public class HrRegisterResponse
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName{ get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty; 

}
