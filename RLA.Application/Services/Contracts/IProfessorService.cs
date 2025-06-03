using RLA.Application.DTOs.UserDTOs;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELP.Services.Contracts
{
    public interface IProfessorService
    {
        Task<Professor> CreateProfessorAsync(AddProfessorDto professorDto);
        Task<Professor> GetProfessorByUserIdAsync(Guid userId);
        Task<IEnumerable<Professor>> GetAllProfessorsAsync();
        Task UpdateProfessorAsync(Guid userId, UpdateUserDto updateDto);
        Task DeleteProfessorAsync(Guid userId);
    }
}