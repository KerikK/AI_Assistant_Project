using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Identity;

namespace DAL
{
    public class AIContext(DbContextOptions<AIContext> opts) : DbContext(opts)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //todo: setup the fluent api
        }
    }
}
