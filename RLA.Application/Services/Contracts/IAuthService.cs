using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLA.Application.DTOs.AuthDTOs;
using RLA.Application.DTOs;
using RLA.Domain.Entities;

namespace RLA.Application.Services.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDTO loginDto);
        Task<ApplicationUser> RegisterStudentAsync(RegisterStudentDto registerDto);
        Task<ApplicationUser> RegisterProfessorAsync(RegisterProfessorDto registerDto);
        Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user, IEnumerable<string> roles);
        Task ConfirmEmailAsync(Guid userId, string token);
        Task<TwoFactorSetupDto> EnableTwoFactorAsync(Guid userId);
        Task<AuthResponseDto> VerifyTwoFactorAsync(string email, string code);
        Task<AuthResponseDto> ExternalLoginAsync(ExternalLoginDto externalDto);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto resetDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(Guid userId);
    }
}
