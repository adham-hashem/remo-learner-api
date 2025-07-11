using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface IStudentRepository
    {
        Task<Student> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<IEnumerable<Student>> GetByCourseIdAsync(Guid courseId);
        Task<IEnumerable<Course>> GetEnrolledCoursesAsync(Guid userId);
        Task EnrollInCourseAsync(Guid userId, Guid courseId);
        Task<bool> HasEnrolledCoursesAsync(Guid userId);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Guid userId);
    }
}