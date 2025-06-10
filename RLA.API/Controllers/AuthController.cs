using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RLA.Application.DTOs;
using RLA.Application.DTOs.AuthDTOs;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RLA.Infrastructure.Repositories.Contracts;

namespace RLA.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        private bool IsPasswordComplex(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
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

            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return ErrorResponse("Email and password are required");

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"Failed login attempt for email: {loginDto.Email}");
                return ErrorResponse("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Failed login attempt for user: {user.Id}");
                return ErrorResponse("Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenService.GenerateToken(user, roles);

            _logger.LogInformation($"Successful login for user: {user.Id}");
            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                FullName = user.FullName,
                Roles = roles
            });
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto registerDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid input data");

            if (!IsPasswordComplex(registerDto.Password))
                return ErrorResponse("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character");

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return ErrorResponse("Email is already in use");

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NationalId = registerDto.NationalId,
                UniversityId = registerDto.UniversityId,
                FullName = registerDto.FullName,
                BirthDate = registerDto.BirthDate,
                Address = registerDto.Address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to create student user: {errors}");
                return ErrorResponse($"Failed to create user: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new Student { UserId = user.Id };
            try
            {
                var studentRepository = HttpContext.RequestServices.GetService<IStudentRepository>();
                if (studentRepository == null)
                    throw new InvalidOperationException("Student repository not registered");

                await studentRepository.AddAsync(student);
                await _userManager.AddToRoleAsync(user, "Student");

                var token = _jwtTokenService.GenerateToken(user, new[] { "Student" });
                _logger.LogInformation($"Successfully registered student: {user.Id}");

                return CreatedAtAction(nameof(Login), new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Roles = new[] { "Student" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create student record for user: {user.Id}");
                await _userManager.DeleteAsync(user);
                return ErrorResponse($"Failed to create student record: {ex.Message}");
            }
        }

        [HttpPost("register/professor")]
        public async Task<IActionResult> RegisterProfessor([FromBody] RegisterProfessorDto registerDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid input data");

            if (!IsPasswordComplex(registerDto.Password))
                return ErrorResponse("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character");

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return ErrorResponse("Email is already in use");

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NationalId = registerDto.NationalId,
                UniversityId = registerDto.UniversityId,
                FullName = registerDto.FullName,
                BirthDate = registerDto.BirthDate,
                Address = registerDto.Address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to create professor user: {errors}");
                return ErrorResponse($"Failed to create user: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Professor");

            var professor = new Professor { UserId = user.Id };
            try
            {
                var professorRepository = HttpContext.RequestServices.GetService<IProfessorRepository>();
                if (professorRepository == null)
                    throw new InvalidOperationException("Professor repository not registered");

                await professorRepository.AddAsync(professor);

                var token = _jwtTokenService.GenerateToken(user, new[] { "Professor" });
                _logger.LogInformation($"Successfully registered professor: {user.Id}");

                return CreatedAtAction(nameof(Login), new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Roles = new[] { "Professor" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create professor record for user: {user.Id}");
                await _userManager.DeleteAsync(user);
                return ErrorResponse($"Failed to create professor record: {ex.Message}");
            }
        }

        //[HttpPost("register/assistant")]
        //public async Task<IActionResult> RegisterAssistant([FromBody] RegisterAssistantDto registerDto)
        //{
        //    if (!ModelState.IsValid)
        //        return ErrorResponse("Invalid input data");

        //    if (!IsPasswordComplex(registerDto.Password))
        //        return ErrorResponse("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character");

        //    var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        //    if (existingUser != null)
        //        return ErrorResponse("Email is already in use");

        //    var user = new ApplicationUser
        //    {
        //        UserName = registerDto.Email,
        //        Email = registerDto.Email,
        //        NationalId = registerDto.NationalId,
        //        UniversityId = registerDto.UniversityId,
        //        FullName = registerDto.FullName,
        //        BirthDate = registerDto.BirthDate,
        //        Address = registerDto.Address,
        //        EmailConfirmed = true
        //    };

        //    var result = await _userManager.CreateAsync(user, registerDto.Password);
        //    if (!result.Succeeded)
        //    {
        //        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //        _logger.LogError($"Failed to create assistant user: {errors}");
        //        return ErrorResponse($"Failed to create user: {errors}");
        //    }

        //    await _userManager.AddToRoleAsync(user, "Assistant");

        //    var assistant = new Assistant { UserId = user.Id };
        //    try
        //    {
        //        var assistantRepository = HttpContext.RequestServices.GetService<IAssistantRepository>();
        //        if (assistantRepository == null)
        //            throw new InvalidOperationException("Assistant repository not registered");

        //        await assistantRepository.AddAsync(assistant);

        //        var token = _jwtTokenService.GenerateToken(user, new[] { "Assistant" });
        //        _logger.LogInformation($"Successfully registered assistant: {user.Id}");

        //        return CreatedAtAction(nameof(Login), new AuthResponseDto
        //        {
        //            Token = token,
        //            UserId = user.Id,
        //            FullName = user.FullName,
        //            Roles = new[] { "Assistant" }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Failed to create assistant record for user: {user.Id}");
        //        await _userManager.DeleteAsync(user);
        //        return ErrorResponse($"Failed to create assistant record: {ex.Message}");
        //    }
        //}
    }
}