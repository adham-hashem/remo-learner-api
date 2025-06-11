using RLA.Infrastructure.Data;
using RLA.Application.DTOs.UserDTOs;
using RLA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using RLA.Application.Services.Contracts;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RLA.Infrastructure.Repositories.Implementations;

namespace RLA.Application.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        public async Task<Student> CreateStudentAsync(AddStudentDto studentDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(studentDto.Email);
            if (existingUser != null)
                throw new Exception("Email is already in use.");

            var user = new ApplicationUser
            {
                UserName = studentDto.Email,
                Email = studentDto.Email,
                FullName = studentDto.FullName,
                UniversityId = studentDto.UniversityId,
                PhoneNumber = studentDto.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, studentDto.Password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new Student { UserId = user.Id };
            await _studentRepository.AddAsync(student);

            return student;
        }

        public async Task<Student> GetStudentByUserIdAsync(Guid userId)
        {
            return await _studentRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(Guid courseId)
        {
            return await _studentRepository.GetByCourseIdAsync(courseId);
        }

        public async Task<IEnumerable<Course>> GetEnrolledCoursesAsync(Guid userId)
        {
            return await _studentRepository.GetEnrolledCoursesAsync(userId);
        }

        public async Task EnrollInCourseAsync(Guid userId, Guid courseId)
        {
            var student = await _studentRepository.GetByUserIdAsync(userId);
            if (student == null)
                throw new Exception("Student not found.");

            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found.");

            await _studentRepository.EnrollInCourseAsync(userId, courseId);
        }

        public async Task UpdateStudentAsync(Guid userId, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found.");

            user.Email = updateDto.Email;
            user.FullName = updateDto.FullName;
            user.UniversityId = updateDto.UniversityId;
            user.PhoneNumber = updateDto.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception($"Failed to update user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);
                if (!passwordResult.Succeeded)
                    throw new Exception($"Failed to reset password: {string.Join(", ", passwordResult.Errors.Select(e => e.Description))}");
            }
        }

        public async Task DeleteStudentAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found.");

            var hasCourses = await _studentRepository.GetByUserIdAsync(userId);
            var studentsInCourses = await _studentRepository.GetByCourseIdAsync(userId);
            if (hasCourses != null && studentsInCourses.Any())
                throw new Exception("Cannot delete student because they are registered in one or more courses.");

            await _studentRepository.DeleteAsync(userId);
            await _userManager.DeleteAsync(user);
        }
    }
}