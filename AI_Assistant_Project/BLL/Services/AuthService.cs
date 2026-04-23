using BLL.Interfaces;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthService(IJwtService jwtService) : IAuthService
    {
        public Task<(bool Success, string Message, AuthResponse? response)> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
