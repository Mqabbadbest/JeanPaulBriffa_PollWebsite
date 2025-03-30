using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace DataAccess.DataContext
{
    public class PollContext: IdentityDbContext<IdentityUser>
    {
        public PollContext(DbContextOptions<PollContext> options) : base(options)
        {
        }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vote>()
                .HasKey(v => new { v.PollId, v.UserId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
