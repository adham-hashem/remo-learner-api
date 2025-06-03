using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class CourseDiscussionRepository : ICourseDiscussionRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public CourseDiscussionRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<CourseDiscussion> GetByIdAsync(Guid id)
        {
            return await _context.CourseDiscussions
                .Include(d => d.Professor)
                .ThenInclude(p => p.User)
                .Include(d => d.Course)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<CourseDiscussion>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseDiscussions
                .Where(d => d.CourseId == courseId)
                .Include(d => d.Professor)
                .ThenInclude(p => p.User)
                .Include(d => d.Course)
                .OrderByDescending(d => d.PostedAt)
                .ToListAsync();
        }

        public async Task AddAsync(CourseDiscussion discussion)
        {
            await _context.CourseDiscussions.AddAsync(discussion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseDiscussion discussion)
        {
            _context.CourseDiscussions.Update(discussion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var discussion = await _context.CourseDiscussions.FindAsync(id);
            if (discussion != null)
            {
                _context.CourseDiscussions.Remove(discussion);
                await _context.SaveChangesAsync();
            }
        }
    }
}