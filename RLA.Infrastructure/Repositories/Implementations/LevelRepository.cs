using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class LevelRepository : ILevelRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public LevelRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Level> GetByIdAsync(Guid id)
        {
            return await _context.Levels.FindAsync(id);
        }

        public async Task<IEnumerable<Level>> GetAllAsync()
        {
            return await _context.Levels.ToListAsync();
        }

        public async Task AddAsync(Level level)
        {
            await _context.Levels.AddAsync(level);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Level level)
        {
            _context.Levels.Update(level);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level != null)
            {
                _context.Levels.Remove(level);
                await _context.SaveChangesAsync();
            }
        }
    }
}