using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface IQuizRepository
    {
        Task<Quiz> GetByIdAsync(Guid id);
        Task<IEnumerable<Quiz>> GetByCourseIdAsync(Guid courseId);
        Task AddAsync(Quiz quiz);
        Task UpdateAsync(Quiz quiz);
        Task DeleteAsync(Guid id);
    }
}