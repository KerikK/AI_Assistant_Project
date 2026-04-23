using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is empty")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is empty")]
        public string Password { get; set; } = string.Empty;
    }
}
