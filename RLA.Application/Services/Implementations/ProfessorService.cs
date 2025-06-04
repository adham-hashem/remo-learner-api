using RLA.Infrastructure.Data;
using RLA.Application.DTOs.UserDTOs;
using RLA.Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfessorService(IProfessorRepository professorRepository, UserManager<ApplicationUser> userManager)
        {
            _professorRepository = professorRepository;
            _userManager = userManager;
        }

        public async Task<Professor> CreateProfessorAsync(AddProfessorDto professorDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(professorDto.Email);
            if (existingUser != null)
                throw new Exception("Email is already in use.");

            var user = new ApplicationUser
            {
                UserName = professorDto.Email,
                Email = professorDto.Email,
                FullName = professorDto.FullName,
                UniversityId = professorDto.UniversityId,
                PhoneNumber = professorDto.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, professorDto.Password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, "Professor");

            var professor = new Professor { UserId = user.Id };
            await _professorRepository.AddAsync(professor);

            return professor;
        }

        public async Task<Professor> GetProfessorByUserIdAsync(Guid userId)
        {
            return await _professorRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Professor>> GetAllProfessorsAsync()
        {
            return await _professorRepository.GetAllAsync();
        }

        public async Task UpdateProfessorAsync(Guid userId, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found.");

            user.Email = updateDto.Email;
            user.FullName = updateDto.FullName;
            user.UniversityId = updateDto.UniversityId;
            user.PhoneNumber = updateDto.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception($"Failed to update user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);
                if (!passwordResult.Succeeded)
                    throw new Exception($"Failed to reset password: {string.Join(", ", passwordResult.Errors.Select(e => e.Description))}");
            }
        }

        public async Task DeleteProfessorAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found.");

            await _professorRepository.DeleteAsync(userId);
            await _userManager.DeleteAsync(user);
        }
    }
}