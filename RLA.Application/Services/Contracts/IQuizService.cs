using RLA.Application.DTOs.QuizzesDTOs;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface IQuizService
    {
        Task<Quiz> CreateQuizAsync(Guid courseId, CreateQuizDto quizDto);
        Task<Quiz> GetQuizByIdAsync(Guid id);
        Task<IEnumerable<Quiz>> GetQuizzesByCourseIdAsync(Guid courseId);
        Task UpdateQuizAsync(Guid id, CreateQuizDto quizDto);
        Task DeleteQuizAsync(Guid id);
    }
}