
namespace Final_Project.Entities
{
    public class Hr
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty; // Login Fields
        
        public string PasswordHash { get; set; } = string.Empty; // Login Fields

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public ICollection<Job>? Jobs { get; set; }
    }
}