using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public ProfessorRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Professor> GetByUserIdAsync(Guid userId)
        {
            return await _context.Professors
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Professor>> GetAllAsync()
        {
            return await _context.Professors
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task AddAsync(Professor professor)
        {
            await _context.Professors.AddAsync(professor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Professor professor)
        {
            _context.Professors.Update(professor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid userId)
        {
            var professor = await _context.Professors.FirstOrDefaultAsync(p => p.UserId == userId);
            if (professor != null)
            {
                _context.Professors.Remove(professor);
                await _context.SaveChangesAsync();
            }
        }
    }
}