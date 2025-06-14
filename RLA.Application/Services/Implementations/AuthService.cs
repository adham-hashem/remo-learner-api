using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using OtpNet;
using RLA.Application.DTOs.AuthDTOs;
using RLA.Application.DTOs;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RLA.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
        }

        private bool IsPasswordComplex(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                throw new Exception("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new Exception("Invalid email or password");

            if (user.TwoFactorEnabled && !string.IsNullOrEmpty(user.TwoFactorSecret))
                throw new Exception("2FA required");

            var roles = await _userManager.GetRolesAsync(user);
            return await GenerateAuthResponseAsync(user, roles);
        }

        public async Task<ApplicationUser> RegisterStudentAsync(RegisterStudentDto registerDto)
        {
            if (!IsPasswordComplex(registerDto.Password))
                throw new Exception("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character");

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new Exception("Email is already in use");

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NationalId = registerDto.NationalId,
                UniversityId = registerDto.UniversityId,
                FullName = registerDto.FullName,
                BirthDate = registerDto.BirthDate,
                Address = registerDto.Address
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, "Student");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{Environment.GetEnvironmentVariable("AppUrl")}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";
            await _emailService.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

            return user;
        }

        public async Task<ApplicationUser> RegisterProfessorAsync(RegisterProfessorDto registerDto)
        {
            if (!IsPasswordComplex(registerDto.Password))
                throw new Exception("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character");

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new Exception("Email is already in use");

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NationalId = registerDto.NationalId,
                UniversityId = registerDto.UniversityId,
                FullName = registerDto.FullName,
                BirthDate = registerDto.BirthDate,
                Address = registerDto.Address
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, "Professor");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{Environment.GetEnvironmentVariable("AppUrl")}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";
            await _emailService.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

            return user;
        }

        public async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var token = _jwtTokenService.GenerateToken(user, roles);
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                FullName = user.FullName,
                Roles = roles,
                RefreshToken = refreshToken
            };
        }

        public async Task ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
                throw new Exception($"Failed to confirm email: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<TwoFactorSetupDto> EnableTwoFactorAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            if (user.TwoFactorEnabled)
                throw new Exception("2FA is already enabled");

            var secretKey = KeyGeneration.GenerateRandomKey(20);
            user.TwoFactorSecret = Base32Encoding.ToString(secretKey);
            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);

            var issuer = Environment.GetEnvironmentVariable("AppName") ?? "RLA";
            var qrCodeUri = $"otpauth://totp/{issuer}:{user.Email}?secret={user.TwoFactorSecret}&issuer={issuer}";

            return new TwoFactorSetupDto
            {
                UserId = user.Id,
                SecretKey = user.TwoFactorSecret,
                QrCodeUri = qrCodeUri
            };
        }

        public async Task<AuthResponseDto> VerifyTwoFactorAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                throw new Exception("2FA not enabled or invalid user");

            var secretKey = Base32Encoding.ToBytes(user.TwoFactorSecret);
            var totp = new Totp(secretKey);
            if (!totp.VerifyTotp(code, out _, new VerificationWindow(1)))
                throw new Exception("Invalid 2FA code");

            var roles = await _userManager.GetRolesAsync(user);
            return await GenerateAuthResponseAsync(user, roles);
        }

        public async Task<AuthResponseDto> ExternalLoginAsync(ExternalLoginDto externalDto)
        {
            var info = new ExternalLoginInfo(null, externalDto.Provider, externalDto.ProviderKey, externalDto.Provider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = externalDto.Email,
                    Email = externalDto.Email,
                    FullName = externalDto.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                await _userManager.AddLoginAsync(user, info);
                await _userManager.AddToRoleAsync(user, "Student");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return await GenerateAuthResponseAsync(user, roles);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{Environment.GetEnvironmentVariable("AppUrl")}/api/auth/reset-password?email={email}&token={encodedToken}";
            await _emailService.SendEmailAsync(email, "Reset your password", $"Reset your password by clicking <a href='{resetLink}'>here</a>.");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetDto)
        {
            var user = await _userManager.FindByEmailAsync(resetDto.Email);
            if (user == null)
                throw new Exception("Invalid email");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetDto.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetDto.NewPassword);
            if (!result.Succeeded)
                throw new Exception($"Failed to reset password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
            if (user == null)
                throw new Exception("Invalid or expired refresh token");

            var roles = await _userManager.GetRolesAsync(user);
            return await GenerateAuthResponseAsync(user, roles);
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
                await _signInManager.SignOutAsync();
            }
        }
    }
}
