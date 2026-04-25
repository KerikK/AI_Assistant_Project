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
    public class UserService : Service<User>, IUserService
    {
        private User? _currentUser = null;

        public UserService(IUserRepository repository) : base(repository) { }

        public User? GetCurrentUser()
            => _currentUser;

        public void SetCurrentUser(User user)
            => _currentUser = user;
    }
}
