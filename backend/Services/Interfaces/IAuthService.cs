using Final_Project.Entities;
using Final_Project.Services.Requests;

namespace Final_Project.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Hr?> RegisterHrAsync(HrRegisterRequest request);
        Task<Candidate?> RegisterCandidateAsync(CandidateRegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
    }
}