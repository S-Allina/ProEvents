
using System.ComponentModel.DataAnnotations;

namespace ProEvent.Domain.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public int MaxParticipants { get; set; }

        public byte[]? Image { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        public Event() { }

        public Event(string name, string description, DateTime date, string location, string category, int maxParticipants)
        {
            Name = name;
            Description = description;
            Date = date;
            Location = location;
            Category = category;
            MaxParticipants = maxParticipants;
        }
    }
}
