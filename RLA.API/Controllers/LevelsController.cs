using RLA.Application.DTOs.LevelsDtos;
using RLA.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RLA.Application.Controllers
{
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLevel([FromBody] LevelRequestDto levelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var level = await _levelService.CreateLevelAsync(levelDto);
                return CreatedAtAction(nameof(GetLevel), new { id = level.Id }, level);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateLevel(Guid id, [FromBody] LevelRequestDto levelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _levelService.UpdateLevelAsync(id, levelDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteLevel(Guid id)
        {
            try
            {
                await _levelService.DeleteLevelAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetLevel(Guid id)
        {
            var level = await _levelService.GetLevelByIdAsync(id);
            if (level == null)
                return NotFound();
            return Ok(level);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLevels()
        {
            var levels = await _levelService.GetAllLevelsAsync();
            return Ok(levels.Select(l => new { l.Id, l.Number }));
        }
    }
}