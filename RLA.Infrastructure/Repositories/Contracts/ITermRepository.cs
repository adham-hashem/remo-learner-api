using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface ITermRepository
    {
        Task<Term> GetByIdAsync(Guid id);
        Task<IEnumerable<Term>> GetAllAsync();
        Task AddAsync(Term term);
        Task UpdateAsync(Term term);
        Task DeleteAsync(Guid id);
    }
}