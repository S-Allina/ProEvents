using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProEvent.Domain.Models;

namespace ProEvent.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Participant> Participants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrollments)
                .HasForeignKey(e => e.EventId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Participant)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(e => e.ParticipantId);

            modelBuilder.Entity<Participant>()
                .HasOne<ApplicationUser>(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.UserId);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Date);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Category);
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.MaxParticipants);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => e.EventId);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => e.ParticipantId);

            modelBuilder.Entity<Participant>()
                .HasIndex(p => p.UserId)
                .IsUnique();

        }
    }
}