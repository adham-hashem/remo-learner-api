using RLA.Application.DTOs.QuizzesDTOs;
using RLA.Infrastructure.Repositories.Contracts;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ICourseRepository _courseRepository;

        public QuizService(IQuizRepository quizRepository, ICourseRepository courseRepository)
        {
            _quizRepository = quizRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Quiz> CreateQuizAsync(Guid courseId, CreateQuizDto quizDto)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found.");

            var quiz = new Quiz
            {
                CourseId = courseId,
                Title = quizDto.Title,
                MaxScore = quizDto.MaxScore,
                TermId = course.TermId,
                Questions = quizDto.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    Answers = q.Options.Select(o => new Answer { Text = o, IsCorrect = o == q.CorrectAnswer }).ToList()
                }).ToList()
            };

            await _quizRepository.AddAsync(quiz);
            return quiz;
        }

        public async Task<Quiz> GetQuizByIdAsync(Guid id)
        {
            return await _quizRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByCourseIdAsync(Guid courseId)
        {
            return await _quizRepository.GetByCourseIdAsync(courseId);
        }

        public async Task UpdateQuizAsync(Guid id, CreateQuizDto quizDto)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null)
                throw new Exception("Quiz not found.");

            quiz.Title = quizDto.Title;
            quiz.MaxScore = quizDto.MaxScore;
            quiz.Questions = quizDto.Questions.Select(q => new Question
            {
                Text = q.Text,
                Answers = q.Options.Select(o => new Answer { Text = o, IsCorrect = o == q.CorrectAnswer }).ToList()
            }).ToList();

            await _quizRepository.UpdateAsync(quiz);
        }

        public async Task DeleteQuizAsync(Guid id)
        {
            await _quizRepository.DeleteAsync(id);
        }
    }
}