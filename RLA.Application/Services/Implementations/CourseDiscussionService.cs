using RLA.Application.DTOs.CourseDiscussionsDTOs;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class CourseDiscussionService : ICourseDiscussionService
    {
        private readonly ICourseDiscussionRepository _discussionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IProfessorRepository _professorRepository;

        public CourseDiscussionService(
            ICourseDiscussionRepository discussionRepository,
            ICourseRepository courseRepository,
            IProfessorRepository professorRepository)
        {
            _discussionRepository = discussionRepository;
            _courseRepository = courseRepository;
            _professorRepository = professorRepository;
        }

        public async Task<CourseDiscussion> CreateDiscussionAsync(Guid courseId, AddDiscussionDto discussionDto, Guid professorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found.");

            var professor = await _professorRepository.GetByUserIdAsync(professorId);
            if (professor == null)
                throw new Exception("Professor not found.");

            if (course.ProfessorId != professor.Id)
                throw new Exception("You are not authorized to add discussions to this course.");

            var discussion = new CourseDiscussion
            {
                CourseId = courseId,
                ProfessorId = professor.Id,
                Message = discussionDto.Message,
                PostedAt = DateTime.UtcNow
            };

            await _discussionRepository.AddAsync(discussion);
            return discussion;
        }

        public async Task<CourseDiscussion> GetDiscussionByIdAsync(Guid id)
        {
            return await _discussionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CourseDiscussion>> GetDiscussionsByCourseIdAsync(Guid courseId)
        {
            return await _discussionRepository.GetByCourseIdAsync(courseId);
        }

        public async Task UpdateDiscussionAsync(Guid id, AddDiscussionDto discussionDto)
        {
            var discussion = await _discussionRepository.GetByIdAsync(id);
            if (discussion == null)
                throw new Exception("Discussion not found.");

            discussion.Message = discussionDto.Message;
            discussion.PostedAt = DateTime.UtcNow;

            await _discussionRepository.UpdateAsync(discussion);
        }

        public async Task DeleteDiscussionAsync(Guid id)
        {
            await _discussionRepository.DeleteAsync(id);
        }
    }
}