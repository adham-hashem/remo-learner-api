using RLA.Application.DTOs.LevelsDtos;
using RLA.Domain.Entities;
using RLA.Infrastructure.Repositories.Contracts;
using RLA.Application.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class LevelService : ILevelService
    {
        private readonly ILevelRepository _levelRepository;

        public LevelService(ILevelRepository levelRepository)
        {
            _levelRepository = levelRepository;
        }

        public async Task<Level> CreateLevelAsync(LevelRequestDto levelDto)
        {
            var level = new Level { Number = levelDto.Number };
            await _levelRepository.AddAsync(level);
            return level;
        }

        public async Task<Level> GetLevelByIdAsync(Guid id)
        {
            return await _levelRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Level>> GetAllLevelsAsync()
        {
            return await _levelRepository.GetAllAsync();
        }

        public async Task UpdateLevelAsync(Guid id, LevelRequestDto levelDto)
        {
            var level = await _levelRepository.GetByIdAsync(id);
            if (level == null)
                throw new Exception("Level not found.");

            level.Number = levelDto.Number;
            await _levelRepository.UpdateAsync(level);
        }

        public async Task DeleteLevelAsync(Guid id)
        {
            await _levelRepository.DeleteAsync(id);
        }
    }
}