using RLA.Application.DTOs.CoursesDTOs;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface ICourseService
    {
        Task<Course> CreateCourseAsync(CreateCourseDto courseDto);
        Task<Course> GetCourseByIdAsync(Guid id);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByProfessorIdAsync(Guid professorId);
        Task UpdateCourseAsync(Guid id, UpdateCourseDto courseDto);
        Task DeleteCourseAsync(Guid id);
    }
}