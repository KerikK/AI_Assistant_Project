using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Identity;

namespace DAL.Context
{
    public class AIContext(DbContextOptions<AIContext> opts) : IdentityDbContext<User>(opts)
    {
    }
}
