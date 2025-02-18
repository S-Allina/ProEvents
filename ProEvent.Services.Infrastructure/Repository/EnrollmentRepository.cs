using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces.IRepository;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Infrastructure.Data;

namespace ProEvent.Services.Infrastructure.Repository
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public EnrollmentRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipantDTO>> GetParticipantsByEventId(int eventId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var enrollments = await _db.Enrollments
                .Where(e => e.EventId == eventId)
                .Include(e => e.Participant)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ParticipantDTO>>(enrollments.Select(e => e.Participant).ToList());
        }

        public async Task<int?> GetParticipantIdByUserId(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

            return participant?.Id;
        }

        public async Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Enrollment enrollment = _mapper.Map<EnrollmentDTO, Enrollment>(enrollmentDTO);

            if (enrollment.Participant != null)
            {
                _db.Entry(enrollment.Participant).State = EntityState.Detached;
            }

            if (enrollment.Id != 0)
            {
                var existingEnrollment = await _db.Enrollments.FindAsync(new object[] { enrollment.Id }, cancellationToken);
                if (existingEnrollment == null)
                {
                    throw new ArgumentException($"Enrollment with ID {enrollment.Id} not found.");
                }
                _db.Enrollments.Update(enrollment);
            }
            else
            {
                _db.Enrollments.Add(enrollment);
            }

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("При регистрации на событие произошла ошибка.", ex);
            }

            return _mapper.Map<Enrollment, EnrollmentDTO>(enrollment);
        }

        public async Task<bool> DeleteEnrollment(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Enrollment enrollment = await _db.Enrollments.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
                if (enrollment == null)
                {
                    return false;
                }

                _db.Enrollments.Remove(enrollment);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<EnrollmentDTO> GetEnrollmentById(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Enrollment enrollment = await _db.Enrollments
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<EnrollmentDTO>(enrollment);
        }

        public async Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<Enrollment> query = _db.Enrollments.Include(e => e.Participant);

            if (eventId.HasValue)
            {
                query = query.Where(e => e.EventId == eventId);
            }

            List<Enrollment> enrollments = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<Enrollment>, List<EnrollmentDTO>>(enrollments);
        }

        public async Task<List<Enrollment>> GetEnrollmentsForParticipantOnDate(int eventId, int participantId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _db.Enrollments
                .Include(e => e.Event)
                .Where(e =>
                    e.ParticipantId == participantId &&
                    (e.EventId == eventId))
                .ToListAsync(cancellationToken);
        }
    }
}
