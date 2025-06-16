using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RLA.Application.DTOs.AuthDTOs;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RLA.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IStudentService _studentService;
        private readonly IProfessorService _professorService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IStudentService studentService,
            IProfessorService professorService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _studentService = studentService;
            _professorService = professorService;
            _logger = logger;
        }

        private IActionResult ErrorResponse(string message)
        {
            return BadRequest(new { Error = message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid input data");

            try
            {
                var result = await _authService.LoginAsync(loginDto);
                _logger.LogInformation($"Successful login for user: {result.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed login attempt for email: {loginDto.Email}. Error: {ex.Message}");
                return ErrorResponse("Invalid email or password");
            }
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto registerDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid input data");

            try
            {
                var user = await _authService.RegisterStudentAsync(registerDto);
                await _studentService.CreateStudentAsync(new Application.DTOs.UserDTOs.AddStudentDto
                {
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    UniversityId = registerDto.UniversityId,
                    Password = registerDto.Password,
                    NationalId = registerDto.NationalId,
                    BirthDate = registerDto.BirthDate,
                    Address = registerDto.Address
                });

                var authResponse = await _authService.GenerateAuthResponseAsync(user, new[] { "Student" });
                _logger.LogInformation($"Successfully registered student: {user.Id}");

                return CreatedAtAction(nameof(Login), authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to register student for email: {registerDto.Email}");
                return ErrorResponse($"Failed to register student: {ex.Message}");
            }
        }

        [HttpPost("register/professor")]
        public async Task<IActionResult> RegisterProfessor([FromBody] RegisterProfessorDto registerDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid input data");

            try
            {
                var user = await _authService.RegisterProfessorAsync(registerDto);
                await _professorService.CreateProfessorAsync(new Application.DTOs.UserDTOs.AddProfessorDto
                {
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    UniversityId = registerDto.UniversityId,
                    Password = registerDto.Password,
                    NationalId = registerDto.NationalId,
                    BirthDate = registerDto.BirthDate,
                    Address = registerDto.Address
                });

                var authResponse = await _authService.GenerateAuthResponseAsync(user, new[] { "Professor" });
                _logger.LogInformation($"Successfully registered professor: {user.Id}");

                return CreatedAtAction(nameof(Login), authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to register professor for email: {registerDto.Email}");
                return ErrorResponse($"Failed to register professor: {ex.Message}");
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmDto)
        {
            try
            {
                await _authService.ConfirmEmailAsync(confirmDto.UserId, confirmDto.Token);
                _logger.LogInformation($"Email confirmed for user: {confirmDto.UserId}");
                return Ok(new { Message = "Email confirmed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to confirm email for user: {confirmDto.UserId}");
                return ErrorResponse($"Failed to confirm email: {ex.Message}");
            }
        }

        [HttpPost("enable-2fa")]
        [Authorize]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            try
            {
                var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out var userId))
                    return ErrorResponse("User not authenticated");

                var setupInfo = await _authService.EnableTwoFactorAsync(userId);
                _logger.LogInformation($"2FA setup initiated for user: {userId}");
                return Ok(setupInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to enable 2FA for user");
                return ErrorResponse($"Failed to enable 2FA: {ex.Message}");
            }
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorDto verifyDto)
        {
            try
            {
                var result = await _authService.VerifyTwoFactorAsync(verifyDto.Email, verifyDto.Code);
                _logger.LogInformation($"Successful 2FA verification for user: {result.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed 2FA verification for email: {verifyDto.Email}");
                return ErrorResponse($"Invalid 2FA code: {ex.Message}");
            }
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto externalDto)
        {
            try
            {
                var result = await _authService.ExternalLoginAsync(externalDto);
                _logger.LogInformation($"Successful external login for user: {result.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed external login for provider: {externalDto.Provider}");
                return ErrorResponse($"Failed external login: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotDto)
        {
            try
            {
                await _authService.ForgotPasswordAsync(forgotDto.Email);
                _logger.LogInformation($"Password reset initiated for email: {forgotDto.Email}");
                return Ok(new { Message = "Password reset email sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to initiate password reset for email: {forgotDto.Email}");
                return ErrorResponse($"Failed to initiate password reset: {ex.Message}");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                await _authService.ResetPasswordAsync(resetDto);
                _logger.LogInformation($"Password reset successful for email: {resetDto.Email}");
                return Ok(new { Message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to reset password for email: {resetDto.Email}");
                return ErrorResponse($"Failed to reset password: {ex.Message}");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshDto.RefreshToken);
                _logger.LogInformation($"Token refreshed for user: {result.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to refresh token");
                return ErrorResponse($"Failed to refresh token: {ex.Message}");
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out var userId))
                    return ErrorResponse("User not authenticated");

                await _authService.LogoutAsync(userId);
                _logger.LogInformation($"User logged out: {userId}");
                return Ok(new { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to logout user");
                return ErrorResponse($"Failed to logout: {ex.Message}");
            }
        }
    }
}