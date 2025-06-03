using RLA.Application.DTOs.CourseDiscussionsDTOs;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface ICourseDiscussionService
    {
        Task<CourseDiscussion> CreateDiscussionAsync(Guid courseId, AddDiscussionDto discussionDto, Guid professorId);
        Task<CourseDiscussion> GetDiscussionByIdAsync(Guid id);
        Task<IEnumerable<CourseDiscussion>> GetDiscussionsByCourseIdAsync(Guid courseId);
        Task UpdateDiscussionAsync(Guid id, AddDiscussionDto discussionDto);
        Task DeleteDiscussionAsync(Guid id);
    }
}