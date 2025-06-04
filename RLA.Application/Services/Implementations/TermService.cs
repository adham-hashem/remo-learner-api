using RLA.Application.DTOs.TermDTOs;
using RLA.Application.Services.Contracts;
using RLA.Domain.Entities;
using RLA.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Implementations
{
    public class TermService : ITermService
    {
        private readonly ITermRepository _termRepository;

        public TermService(ITermRepository termRepository)
        {
            _termRepository = termRepository;
        }

        public async Task<Term> CreateTermAsync(TermRequestDto termDto)
        {
            var term = new Term
            {
                Name = termDto.Name,
                StartDate = termDto.StartDate,
                EndDate = termDto.EndDate
            };
            await _termRepository.AddAsync(term);
            return term;
        }

        public async Task<Term> GetTermByIdAsync(Guid id)
        {
            return await _termRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Term>> GetAllTermsAsync()
        {
            return await _termRepository.GetAllAsync();
        }

        public async Task UpdateTermAsync(Guid id, TermRequestDto termDto)
        {
            var term = await _termRepository.GetByIdAsync(id);
            if (term == null)
                throw new Exception("Term not found.");

            term.Name = termDto.Name;
            term.StartDate = termDto.StartDate;
            term.EndDate = termDto.EndDate;
            await _termRepository.UpdateAsync(term);
        }

        public async Task DeleteTermAsync(Guid id)
        {
            await _termRepository.DeleteAsync(id);
        }
    }
}