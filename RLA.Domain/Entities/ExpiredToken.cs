using System.ComponentModel.DataAnnotations;

namespace RLA.Domain.Entities
{
    public class ExpiredToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public String? Token { get; set; }
    }
}
