using Final_Project.Services.Interfaces;
using Final_Project.Services.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("hr/register")]
        public async Task<ActionResult<HrRegisterResponse>> Register(HrRegisterRequest request)
        {
            var user = await authService.RegisterHrAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists");
            }

            var response = new HrRegisterResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            return Ok(response);
        }

        [HttpPost("candidate/register")]
        public async Task<ActionResult<CandidateRegisterResponse>> Register(CandidateRegisterRequest request)
        {
            var user = await authService.RegisterCandidateAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists");
            }

            var response = new CandidateRegisterResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(token);
        }
    }
}