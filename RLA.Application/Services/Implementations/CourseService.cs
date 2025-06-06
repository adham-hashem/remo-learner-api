using RLA.Application.DTOs.CoursesDTOs;
using RLA.Domain.Entities;
using RLA.Infrastructure.Repositories.Contracts;
using RLA.Application.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly ILevelRepository _levelRepository;
        private readonly ITermRepository _termRepository;

        public CourseService(
            ICourseRepository courseRepository,
            IProfessorRepository professorRepository,
            ILevelRepository levelRepository,
            ITermRepository termRepository)
        {
            _courseRepository = courseRepository;
            _professorRepository = professorRepository;
            _levelRepository = levelRepository;
            _termRepository = termRepository;
        }

        public async Task<Course> CreateCourseAsync(CreateCourseDto courseDto)
        {
            var professorExists = await _professorRepository.GetByUserIdAsync(courseDto.ProfessorId);
            if (professorExists == null)
                throw new Exception($"No professor found with ID {courseDto.ProfessorId}.");

            var termExists = await _termRepository.GetByIdAsync(courseDto.TermId);
            if (termExists == null)
                throw new Exception($"No term found with ID {courseDto.TermId}.");

            var levelExists = await _levelRepository.GetByIdAsync(courseDto.LevelId);
            if (levelExists == null)
                throw new Exception($"No level found with ID {courseDto.LevelId}.");

            var course = new Course
            {
                Name = courseDto.Name,
                Code = courseDto.Code,
                Overview = courseDto.Overview,
                DayOfWeek = courseDto.DayOfWeek,
                Time = courseDto.Time,
                Location = courseDto.Location,
                CreditHours = courseDto.CreditHours,
                LevelId = courseDto.LevelId,
                ProfessorId = courseDto.ProfessorId,
                TermId = courseDto.TermId
            };

            await _courseRepository.AddAsync(course);
            return course;
        }

        public async Task<Course> GetCourseByIdAsync(Guid id)
        {
            return await _courseRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByProfessorIdAsync(Guid professorId)
        {
            return await _courseRepository.GetByProfessorIdAsync(professorId);
        }

        public async Task UpdateCourseAsync(Guid id, UpdateCourseDto courseDto)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new Exception("Course not found.");

            course.Name = courseDto.Name;
            course.Code = courseDto.Code;
            course.Overview = courseDto.Overview;
            course.DayOfWeek = courseDto.DayOfWeek;
            course.Time = courseDto.Time;
            course.Location = courseDto.Location;
            course.CreditHours = courseDto.CreditHours;
            course.LevelId = courseDto.LevelId;
            course.ProfessorId = courseDto.ProfessorId;
            course.TermId = courseDto.TermId;

            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            await _courseRepository.DeleteAsync(id);
        }
    }
}