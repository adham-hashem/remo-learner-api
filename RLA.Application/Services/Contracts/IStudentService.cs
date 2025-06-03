using RLA.Application.DTOs.UserDTOs;
using RLA.Domain.Entities;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface IStudentService
    {
        Task<Student> CreateStudentAsync(AddStudentDto studentDto);
        Task<Student> GetStudentByUserIdAsync(Guid userId);
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(Guid courseId);
        Task UpdateStudentAsync(Guid userId, UpdateUserDto updateDto);
        Task DeleteStudentAsync(Guid userId);
    }
}