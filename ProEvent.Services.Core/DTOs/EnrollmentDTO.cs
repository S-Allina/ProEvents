using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.DTOs
{
    public class EnrollmentDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int? ParticipantId { get; set; }
        public string? UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Participant Participant { get; set; }


    }
}
