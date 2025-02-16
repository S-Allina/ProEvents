using ProEvent.Services.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Core.Interfaces.IService
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken);
        Task<bool> DeleteEnrollment(int id, CancellationToken cancellationToken);
        Task<bool> CanParticipantAttendEvent(int participantId, int eventId, CancellationToken cancellationToken);
        Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null, CancellationToken cancellationToken = default);
        Task<EnrollmentDTO> GetEnrollmentById(int id, CancellationToken cancellationToken);
        Task<List<ParticipantWithRegistrationDTO>> GetEnrollmentsWithParticipantInfo(int? eventId = null, CancellationToken cancellationToken = default);
    }
}
