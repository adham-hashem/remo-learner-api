using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public QuizRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Quiz> GetByIdAsync(Guid id)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Quizzes
                .Where(q => q.CourseId == courseId)
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .Include(q => q.Course)
                .ToListAsync();
        }

        public async Task AddAsync(Quiz quiz)
        {
            await _context.Quizzes.AddAsync(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
        }
    }
}