using AI_Assistant_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<IEnumerable<Request>> GetByUserIdAsync(string userId);
    }
}
