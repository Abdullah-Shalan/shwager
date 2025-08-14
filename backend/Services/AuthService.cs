using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Final_Project.DAL;
using Final_Project.DAL.Repositories;
using Final_Project.Entities;
using Final_Project.Services.Interfaces;
using Final_Project.Services.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Final_Project.Services;

public class AuthService : IAuthService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(UnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var hrUser = await _unitOfWork.Hrs.GetFirstOrDefaultAsync(u => u.Email == request.Email);
        if (hrUser != null)
        {
            // Hr email found, check password
            if (new PasswordHasher<Hr>().VerifyHashedPassword(hrUser, hrUser.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return GetToken(hrUser);
        }

        var candidateUser = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(u => u.Email == request.Email);
        if (candidateUser != null)
        {
            if (new PasswordHasher<Candidate>().VerifyHashedPassword(candidateUser, candidateUser.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return GetToken(candidateUser);
        }

        return null; // Email not in Hrs or Candidates
    }

    public async Task<Candidate?> RegisterCandidateAsync(CandidateRegisterRequest request)
    {

        var inCandidates = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(u => u.Email == request.Email);
        var inHrs = await _unitOfWork.Hrs.GetFirstOrDefaultAsync(u => u.Email == request.Email);

        if (inCandidates != null || inHrs != null)
        {
            return null;
        }

        var candidate = new Candidate();

        var hashedPassword = new PasswordHasher<Candidate>()
        .HashPassword(candidate, request.Password);

        candidate.Email = request.Email;
        candidate.PasswordHash = hashedPassword;
        candidate.FirstName = request.FirstName;
        candidate.LastName = request.LastName;
        candidate.ResumeUrl = request.ResumeUrl;

        await _unitOfWork.Candidates.InsertAsync(candidate);
        await _unitOfWork.SaveAsync();

        return candidate;

    }

    public async Task<Hr?> RegisterHrAsync(HrRegisterRequest request)
    {
        var inCandidates = await _unitOfWork.Candidates.GetFirstOrDefaultAsync(u => u.Email == request.Email);
        var inHrs = await _unitOfWork.Hrs.GetFirstOrDefaultAsync(u => u.Email == request.Email);

        if (inCandidates != null || inHrs != null)
        {
            return null;
        }

        var hr = new Hr();

        var hashedPassword = new PasswordHasher<Hr>()
        .HashPassword(hr, request.Password);

        hr.Email = request.Email;
        hr.PasswordHash = hashedPassword;
        hr.FirstName = request.FirstName;
        hr.LastName = request.LastName;

        await _unitOfWork.Hrs.InsertAsync(hr);
        await _unitOfWork.SaveAsync();

        return hr;
    }

    private string? GetToken(Hr hr)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, hr.Id.ToString()),
                new Claim(ClaimTypes.Name, string.Concat(hr.FirstName," ", hr.LastName)),
                new Claim(ClaimTypes.Role, "Hr"),
            };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private string? GetToken(Candidate candidate)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, candidate.Id.ToString()),
                new Claim(ClaimTypes.Name, string.Concat(candidate.FirstName," ", candidate.LastName)),
                new Claim(ClaimTypes.Role, "Candidate"),
            };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }


}