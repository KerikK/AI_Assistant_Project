using DAL.Interfaces;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AIContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task RevokeAllUserTokens(int userId)
        {
            var tokens = await _dbSet.Where(t => t.UserId == userId && !t.IsRevoked).ToListAsync();
            tokens.ForEach(t => { t.IsRevoked = true; });
        }
    }
}
