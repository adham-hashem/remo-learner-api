using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public MaterialRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Material> GetByIdAsync(Guid id)
        {
            return await _context.Materials
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Material>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Materials
                .Where(m => m.CourseId == courseId)
                .Include(m => m.Course)
                .OrderBy(m => m.WeekNumber)
                .ToListAsync();
        }

        public async Task AddAsync(Material material)
        {
            await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Material material)
        {
            _context.Materials.Update(material);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
            }
        }
    }
}