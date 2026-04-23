using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthService(IUserRepository userRepository, IJwtService jwtService) : IAuthService
    {
        public async Task<(bool Success, string Message, AuthResponse? response)> LoginAsync(LoginRequest request)
        {
            var user = await userRepository.GetByUserNameAsync(request.UserName);
            if (user == null)
                return (false, "User not found", null);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return (false, "Incorrect password", null);

            var response = await CreateAuthResponseAsync(user);

            return (true, "Success", response);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {

            if (await userRepository.AnyAsync(u => u.UserName == request.UserName))
                return (false, "This user already exists");

            if (await userRepository.AnyAsync(u => u.Email == request.Email))
                return (false, "This email is already taken");

            var user = new User()
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            await userRepository.CreateAsync(user);
            await userRepository.SaveChangeAsync();

            return (true, "Register succeeded, now login");
        }

        private async Task<AuthResponse> CreateAuthResponseAsync(User user)
        {
            var accessToken = jwtService.GenerateToken(user);
            return new AuthResponse()
            {
                AccessToken = accessToken,
                UserName = user.UserName,
                Email = user.Email,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };
        }
    }
}
