
using ProEvent.Domain.Enums;

namespace ProEvent.BLL.DTOs
{
    public class EventWithRegistrationDTO
    {
        public int ParticipanId { get; set; }
        public int EnrollmentId { get; set; }
        public int EventId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public int MaxParticipants { get; set; }

        public EventStatus Status { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
