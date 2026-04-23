using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL.Interfaces
{
    public interface IAuthService
    {
        public Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
        public Task<(bool Success, string Message, AuthResponse? response)> LoginAsync(LoginRequest request);
    }
}
