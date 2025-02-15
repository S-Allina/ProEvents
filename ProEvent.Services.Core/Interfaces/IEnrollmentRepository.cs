
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Repository
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetEnrollmentsForParticipantOnDate(int eventId, int participantId);
        Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null);
            Task<int?> GetParticipantIdByUserId(string userId);
        Task<EnrollmentDTO> GetEnrollmentById(int id);
        Task<IEnumerable<ParticipantDTO>> GetParticipantsByEventId(int eventId);
        Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO);
        Task<bool> DeleteEnrollment(int id);
    }
}
