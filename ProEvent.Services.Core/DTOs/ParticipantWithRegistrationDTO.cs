namespace ProEvent.Services.Core.DTOs
{
    public class ParticipantWithRegistrationDTO
    {
        public int ParticipanId { get; set; } 
        public int EnrollmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
