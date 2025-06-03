using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface IProfessorRepository
    {
        Task<Professor> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Professor>> GetAllAsync();
        Task AddAsync(Professor professor);
        Task UpdateAsync(Professor professor);
        Task DeleteAsync(Guid userId);
    }
}