using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is empty")]
        [MinLength(3)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is empty")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is empty")]
        [MinLength(5)]
        public string Password { get; set; } = string.Empty;
    }
}
