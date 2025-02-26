namespace ProEvent.Domain.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int ParticipantId { get; set; }
        public Participant Participant { get; set; }
        public DateTime RegistrationDate { get; set; } 
    }
}
