using RLA.Application.DTOs.LevelsDtos;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface ILevelService
    {
        Task<Level> CreateLevelAsync(LevelRequestDto levelDto);
        Task<Level> GetLevelByIdAsync(Guid id);
        Task<IEnumerable<Level>> GetAllLevelsAsync();
        Task UpdateLevelAsync(Guid id, LevelRequestDto levelDto);
        Task DeleteLevelAsync(Guid id);
    }
}