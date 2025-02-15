using ProEvent.Services.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Core.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO);
        Task<bool> DeleteEnrollment(int id);
        Task<bool> CanParticipantAttendEvent(int participantId,int eventId);
        Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null);
        Task<EnrollmentDTO> GetEnrollmentById(int id);
        Task<List<ParticipantWithRegistrationDTO>> GetEnrollmentsWithParticipantInfo(int? eventId = null);

    }
}
