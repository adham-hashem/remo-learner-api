using RLA.Application.DTOs.MaterialsDTOs;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface IMaterialService
    {
        Task<Material> CreateMaterialAsync(Guid courseId, AddMaterialDto materialDto);
        Task<Material> GetMaterialByIdAsync(Guid id);
        Task<IEnumerable<Material>> GetMaterialsByCourseIdAsync(Guid courseId);
        Task UpdateMaterialAsync(Guid id, AddMaterialDto materialDto);
        Task DeleteMaterialAsync(Guid id);
    }
}