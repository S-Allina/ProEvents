using ProEvent.Domain.Models;
using System.Text.Json.Serialization;

namespace ProEvent.BLL.DTOs
{
    public class EnrollmentDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int? ParticipantId { get; set; }
        public string? UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        [JsonIgnore]
        public Participant? Participant { get; set; }
    }
}
