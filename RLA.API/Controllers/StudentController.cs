using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RLA.Application.DTOs.UserDTOs;
using RLA.Application.DTOs.CoursesDTOs;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace RLA.API.Controllers
{
    [ApiController]
    [Route("api/students")]
    // [Authorize(Roles = "Student,Admin")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IStudentService studentService,
            ICourseService courseService,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentController> logger)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IActionResult ErrorResponse(string message)
        {
            return BadRequest(new { Error = message });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdString = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("Profile access attempted without valid authentication");
                    return ErrorResponse("User not authenticated");
                }

                var student = await _studentService.GetStudentByUserIdAsync(userId);
                if (student == null || student.User == null)
                {
                    _logger.LogWarning($"Student profile not found for user ID: {userId}");
                    return ErrorResponse("Student profile not found");
                }

                var profileDto = new
                {
                    UserId = student.UserId,
                    Email = student.User.Email,
                    FullName = student.User.FullName,
                    UniversityId = student.User.UniversityId,
                    PhoneNumber = student.User.PhoneNumber
                };

                _logger.LogInformation($"Successfully retrieved profile for student: {userId}");
                return Ok(new { Data = profileDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve profile for user: {User.Identity?.Name}");
                return ErrorResponse("An error occurred while retrieving the profile");
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning($"Invalid input data for profile update: {errors}");
                return ErrorResponse($"Invalid input data: {errors}");
            }

            try
            {
                var userIdString = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("Profile update attempted without valid authentication");
                    return ErrorResponse("User not authenticated");
                }

                var student = await _studentService.GetStudentByUserIdAsync(userId);
                if (student == null)
                {
                    _logger.LogWarning($"Student profile not found for user ID: {userId}");
                    return ErrorResponse("Student profile not found");
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(updateDto.Email) && string.IsNullOrWhiteSpace(updateDto.FullName) &&
                    string.IsNullOrWhiteSpace(updateDto.UniversityId) && string.IsNullOrWhiteSpace(updateDto.PhoneNumber) &&
                    string.IsNullOrWhiteSpace(updateDto.Password))
                {
                    _logger.LogWarning($"No valid fields provided for profile update for user ID: {userId}");
                    return ErrorResponse("At least one field must be provided for update");
                }

                await _studentService.UpdateStudentAsync(userId, updateDto);
                _logger.LogInformation($"Successfully updated profile for student: {userId}");
                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update profile for user: {User.Identity?.Name}");
                return ErrorResponse($"Failed to update profile: {ex.Message}");
            }
        }

        [HttpGet("courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetEnrolledCourses()
        {
            try
            {
                var userIdString = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("Course retrieval attempted without valid authentication");
                    return ErrorResponse("User not authenticated");
                }

                var student = await _studentService.GetStudentByUserIdAsync(userId);
                if (student == null)
                {
                    _logger.LogWarning($"Student profile not found for user ID: {userId}");
                    return ErrorResponse("Student profile not found");
                }

                var courses = await _studentService.GetEnrolledCoursesAsync(userId);
                var courseDtos = courses.Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Description = c.Overview
                }).ToList();

                _logger.LogInformation($"Successfully retrieved {courseDtos.Count} enrolled courses for student: {userId}");
                return Ok(new { Data = courseDtos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve courses for user: {User.Identity?.Name}");
                return ErrorResponse("An error occurred while retrieving enrolled courses");
            }
        }

        [HttpPost("courses/{courseId}/enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollInCourse(Guid courseId)
        {
            try
            {
                var userIdString = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("Course enrollment attempted without valid authentication");
                    return ErrorResponse("User not authenticated");
                }

                var student = await _studentService.GetStudentByUserIdAsync(userId);
                if (student == null)
                {
                    _logger.LogWarning($"Student profile not found for user ID: {userId}");
                    return ErrorResponse("Student profile not found");
                }

                var course = await _courseService.GetCourseByIdAsync(courseId);
                if (course == null)
                {
                    _logger.LogWarning($"Course not found for ID: {courseId}");
                    return ErrorResponse("Course not found");
                }

                await _studentService.EnrollInCourseAsync(userId, courseId);
                _logger.LogInformation($"Student {userId} successfully enrolled in course {courseId}");
                return Ok(new { Message = "Successfully enrolled in course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to enroll student in course {courseId} for user: {User.Identity?.Name}");
                return ErrorResponse($"Failed to enroll in course: {ex.Message}");
            }
        }
    }
}