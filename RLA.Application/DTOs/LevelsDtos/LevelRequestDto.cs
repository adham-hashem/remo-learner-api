using System.ComponentModel.DataAnnotations;

namespace ELP.Application.DTOs.LevelsDtos
{
    /// <summary>
    /// DTO for creating or updating an academic level.
    /// </summary>
    public class LevelRequestDto
    {
        [Required(ErrorMessage = "Level number is required.")]
        [Range(1, 4, ErrorMessage = "Level number must be between 1 and 4.")]
        public int Number { get; set; }
    }
}