using ProEvent.Domain.Models;

namespace ProEvent.DAL.Interfaces.IRepository
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetEnrollmentsForParticipantOnDate(int eventId, int participantId, CancellationToken cancellationToken);
        Task<IEnumerable<Enrollment>> GetEnrollments(int? eventId = null, CancellationToken cancellationToken = default);
        Task<int?> GetParticipantIdByUserId(string userId, CancellationToken cancellationToken);
        Task<Enrollment> GetEnrollmentById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Participant>> GetParticipantsByEventId(int eventId, CancellationToken cancellationToken);
        Task<Enrollment> CreateUpdateEnrollment(Enrollment enrollment, CancellationToken cancellationToken);
        Task<bool> DeleteEnrollment(int id, CancellationToken cancellationToken);
    }
}
