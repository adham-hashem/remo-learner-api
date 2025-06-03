using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface IMaterialRepository
    {
        Task<Material> GetByIdAsync(Guid id);
        Task<IEnumerable<Material>> GetByCourseIdAsync(Guid courseId);
        Task AddAsync(Material material);
        Task UpdateAsync(Material material);
        Task DeleteAsync(Guid id);
    }
}