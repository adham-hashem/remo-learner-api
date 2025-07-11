using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public StudentRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Student> GetByUserIdAsync(Guid userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                .ThenInclude(s => s.User)
                .Select(sc => sc.Student)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetEnrolledCoursesAsync(Guid userId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.Student.UserId == userId)
                .Include(sc => sc.Course)
                .Select(sc => sc.Course)
                .ToListAsync();
        }

        public async Task EnrollInCourseAsync(Guid userId, Guid courseId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
                throw new Exception("Student not found.");

            var isEnrolled = await _context.StudentCourses
                .AnyAsync(sc => sc.Student.UserId == userId && sc.CourseId == courseId);
            if (isEnrolled)
                throw new Exception("Student is already enrolled in this course.");

            var studentCourse = new StudentCourse
            {
                StudentId = student.Id,
                CourseId = courseId
            };

            await _context.StudentCourses.AddAsync(studentCourse);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasEnrolledCoursesAsync(Guid userId)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.Student.UserId == userId);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid userId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}