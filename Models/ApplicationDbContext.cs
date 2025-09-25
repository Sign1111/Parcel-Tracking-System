using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking.Models;

namespace Parcel_Tracking.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<ReplyViewModel> ReplyViewModel { get; set; }

        public DbSet<Reply> Replies { get; set; }

        public DbSet<UserSessionControl> UserSessionControls { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReplyTo)
                .WithMany(m => m.Replies) // uses the ICollection<Message> Replies above
                .HasForeignKey(m => m.ReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict); // stops cascade delete loops
        }







    }
}