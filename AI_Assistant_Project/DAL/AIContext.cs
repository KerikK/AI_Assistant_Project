using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Identity;
using AI_Assistant_Project.Models;

namespace DAL
{
    public class AIContext(DbContextOptions<AIContext> opts) : DbContext(opts)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AiRequest> Requests { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Requests)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<AiRequest>()
                .HasOne(req => req.Response)
                .WithOne(res => res.Request)
                .HasForeignKey<AiRequest>(req => req.ResponseId);
        }
    }
}
