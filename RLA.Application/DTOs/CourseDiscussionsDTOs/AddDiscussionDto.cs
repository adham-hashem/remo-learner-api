using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.CourseDiscussionsDTOs
{
    /// <summary>
    /// DTO for adding or updating a course discussion post.
    /// </summary>
    public class AddDiscussionDto
    {
        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
        public string Message { get; set; }
    }
}