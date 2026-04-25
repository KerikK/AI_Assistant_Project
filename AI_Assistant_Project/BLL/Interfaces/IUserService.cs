using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService : IService<User>
    {
        public void SetCurrentUser(User user);
        public User? GetCurrentUser();
    }
}
