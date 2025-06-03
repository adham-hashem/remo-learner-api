using Microsoft.EntityFrameworkCore;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Implementations
{
    public class TermRepository : ITermRepository
    {
        private readonly ElearningPlatformDbContext _context;

        public TermRepository(ElearningPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<Term> GetByIdAsync(Guid id)
        {
            return await _context.Terms.FindAsync(id);
        }

        public async Task<IEnumerable<Term>> GetAllAsync()
        {
            return await _context.Terms.ToListAsync();
        }

        public async Task AddAsync(Term term)
        {
            await _context.Terms.AddAsync(term);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Term term)
        {
            _context.Terms.Update(term);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var term = await _context.Terms.FindAsync(id);
            if (term != null)
            {
                _context.Terms.Remove(term);
                await _context.SaveChangesAsync();
            }
        }
    }
}