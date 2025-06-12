using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLA.Application.DTOs.AuthDTOs
{
    public class ExternalLoginDto
    {
        [Required]
        public string Provider { get; set; } = string.Empty;

        [Required]
        public string ProviderKey { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
