using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class AiRequestDto
    {
        [Required(ErrorMessage = "Prompt can not be empty")]
        [MinLength(3)]
        public string Prompt { get; set; } = string.Empty;
    }
}
