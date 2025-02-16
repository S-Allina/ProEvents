using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Interfaces.IRepository
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetEnrollmentsForParticipantOnDate(int eventId, int participantId, CancellationToken cancellationToken);
        Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null, CancellationToken cancellationToken = default);
        Task<int?> GetParticipantIdByUserId(string userId, CancellationToken cancellationToken);
        Task<EnrollmentDTO> GetEnrollmentById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<ParticipantDTO>> GetParticipantsByEventId(int eventId, CancellationToken cancellationToken);
        Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken);
        Task<bool> DeleteEnrollment(int id, CancellationToken cancellationToken);
    }
}
