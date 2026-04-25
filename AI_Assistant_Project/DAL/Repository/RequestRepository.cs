using AI_Assistant_Project.Models;
using Azure.Core;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class RequestRepository : Repository<AiRequest>, IRequestRepository
    {
        public RequestRepository(AIContext context) : base(context) { }

        public async Task<IEnumerable<AiRequest>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public override async Task<bool> AnyAsync(Expression<Func<AiRequest, bool>> pred)
            => await _dbSet.Include(r => r.Response).AnyAsync(pred);

        public override async Task<AiRequest?> FirstOrDefaultAsync(Expression<Func<AiRequest, bool>> pred)
            => await _dbSet.Include(r => r.Response).FirstOrDefaultAsync(pred);
    }
}
