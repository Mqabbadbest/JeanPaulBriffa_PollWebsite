using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace DataAccess.DataContext
{
    public class PollDbContext: IdentityDbContext<IdentityUser>
    {
        public PollDbContext(DbContextOptions<PollDbContext> options) : base(options)
        {
        }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Vote> Votes { get; set; }

        /// <summary>
        /// This method is making sure that the Vote entity has a composite key of PollId and UserId.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vote>()
                .HasKey(v => new { v.PollId, v.UserId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
