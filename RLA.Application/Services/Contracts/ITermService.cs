using RLA.Application.DTOs.TermDTOs;
using RLA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface ITermService
    {
        Task<Term> CreateTermAsync(TermRequestDto termDto);
        Task<Term> GetTermByIdAsync(Guid id);
        Task<IEnumerable<Term>> GetAllTermsAsync();
        Task UpdateTermAsync(Guid id, TermRequestDto termDto);
        Task DeleteTermAsync(Guid id);
    }
}