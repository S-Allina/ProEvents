using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // IdentityDbContext вместо DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Participant> Participants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ВАЖНО: Сначала вызовите базовую реализацию!

            // Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrollments)
                .HasForeignKey(e => e.EventId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Participant)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(e => e.ParticipantId);

            // Связь между Participant и ApplicationUser
            modelBuilder.Entity<Participant>()
                .HasOne<ApplicationUser>(p => p.User) // Навигационное свойство к ApplicationUser
                .WithOne(u => u.Participant) // Навигационное свойство к Participant в ApplicationUser
                .HasForeignKey<Participant>(p => p.UserId); // Внешний ключ в Participant

            // Event Indexes
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Date);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Category);
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.MaxParticipants);

            // Enrollment Indexes
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