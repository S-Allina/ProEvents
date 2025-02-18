using ProEvent.Services.Core.Interfaces;
using System.Text.Json.Serialization;

namespace ProEvent.Services.Core.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public int MaxParticipants { get; set; }

        public byte[]? Image { get; set; }

        public EventStatus? Status { get; set; } = null;

        public EventDTO(string name, string description, byte[]? image, DateTime date, string location, string category, int maxParticipants)
        {
            this.Name = name;
            Description = description;
            Image = image;
            Date = date;
            Location = location;
            Category = category;
            MaxParticipants = maxParticipants;
        }
    }
}
