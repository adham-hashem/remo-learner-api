using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RLA.Application.DTOs.TermDTOs;
using RLA.Application.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace RLA.Application.Controllers
{
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TermsController : ControllerBase
    {
        private readonly ITermService _termService;

        public TermsController(ITermService termService)
        {
            _termService = termService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTerm([FromBody] TermRequestDto termDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var term = await _termService.CreateTermAsync(termDto);
                return CreatedAtAction(nameof(GetTerm), new { id = term.Id }, term);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTerm(Guid id, [FromBody] TermRequestDto termDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _termService.UpdateTermAsync(id, termDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTerm(Guid id)
        {
            try
            {
                await _termService.DeleteTermAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTerm(Guid id)
        {
            var term = await _termService.GetTermByIdAsync(id);
            if (term == null)
                return NotFound();
            return Ok(term);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTerms()
        {
            var terms = await _termService.GetAllTermsAsync();
            return Ok(terms.Select(t => new { t.Id, t.Name, t.StartDate, t.EndDate }));
        }
    }
}