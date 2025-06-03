using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public CourseRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Level)
                .Include(c => c.Term)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Level)
                .Include(c => c.Term)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByProfessorIdAsync(Guid professorId)
        {
            return await _context.Courses
                .Where(c => c.ProfessorId == professorId)
                .Include(c => c.Professor)
                .Include(c => c.Level)
                .Include(c => c.Term)
                .ToListAsync();
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}