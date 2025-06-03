using System.ComponentModel.DataAnnotations;

namespace RLA.Domain.Entities
{
    public class Level
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Range(1, 4, ErrorMessage = "The level number must be a number from 1 to 4")]
        public int Number { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }

}
