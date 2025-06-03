using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface ILevelRepository
    {
        Task<Level> GetByIdAsync(Guid id);
        Task<IEnumerable<Level>> GetAllAsync();
        Task AddAsync(Level level);
        Task UpdateAsync(Level level);
        Task DeleteAsync(Guid id);
    }
}