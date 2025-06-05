using RLA.Application.DTOs.MaterialsDTOs;
using RLA.Domain.Entities;
using RLA.Application.Services.Contracts;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly ICourseRepository _courseRepository;

        public MaterialService(IMaterialRepository materialRepository, ICourseRepository courseRepository)
        {
            _materialRepository = materialRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Material> CreateMaterialAsync(Guid courseId, AddMaterialDto materialDto)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found.");

            var material = new Material
            {
                CourseId = courseId,
                WeekNumber = materialDto.WeekNumber.ToString(),
                LectureTitle = materialDto.LectureTitle,
                LectureDescription = materialDto.LectureDescription,
                FilePath = materialDto.FilePath
            };

            await _materialRepository.AddAsync(material);
            return material;
        }

        public async Task<Material> GetMaterialByIdAsync(Guid id)
        {
            return await _materialRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Material>> GetMaterialsByCourseIdAsync(Guid courseId)
        {
            return await _materialRepository.GetByCourseIdAsync(courseId);
        }

        public async Task UpdateMaterialAsync(Guid id, AddMaterialDto materialDto)
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null)
                throw new Exception("Material not found.");

            material.WeekNumber = materialDto.WeekNumber.ToString();
            material.LectureTitle = materialDto.LectureTitle;
            material.LectureDescription = materialDto.LectureDescription;
            material.FilePath = materialDto.FilePath;

            await _materialRepository.UpdateAsync(material);
        }

        public async Task DeleteMaterialAsync(Guid id)
        {
            await _materialRepository.DeleteAsync(id);
        }
    }
}