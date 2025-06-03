using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Repositories.Contracts
{
    public interface ICourseDiscussionRepository
    {
        Task<CourseDiscussion> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseDiscussion>> GetByCourseIdAsync(Guid courseId);
        Task AddAsync(CourseDiscussion discussion);
        Task UpdateAsync(CourseDiscussion discussion);
        Task DeleteAsync(Guid id);
    }
}