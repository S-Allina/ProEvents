using AutoMapper;
using FluentValidation;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces.IRepository;
using ProEvent.Services.Core.Interfaces.IService;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Validators;
using ProEvent.Services.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Infrastructure.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IValidator<Enrollment> _enrollmentValidator;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IValidator<Enrollment> enrollmentValidator, IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _enrollmentValidator = enrollmentValidator;
            _mapper = mapper;
        }
        public async Task<List<ParticipantWithRegistrationDTO>> GetEnrollmentsWithParticipantInfo(int? eventId = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<EnrollmentDTO> enrollments = await _enrollmentRepository.GetEnrollments(eventId, cancellationToken);
            return enrollments.Select(enrollment => new ParticipantWithRegistrationDTO
            {
                ParticipanId = enrollment.Participant.Id,
                EnrollmentId = enrollment.Id,
                FirstName = enrollment.Participant.FirstName,
                LastName = enrollment.Participant.LastName,
                DateOfBirth = enrollment.Participant.DateOfBirth,
                Email = enrollment.Participant.Email,
                UserId = enrollment.Participant.UserId,
                RegistrationDate = enrollment.RegistrationDate
            }).ToList();
        }

        public async Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int? participantId = await _enrollmentRepository.GetParticipantIdByUserId(enrollmentDTO.UserId, cancellationToken);
            if (participantId == null)
            {
                throw new ArgumentException("Участник не найден");
            }
            enrollmentDTO.ParticipantId = participantId.Value;
            Enrollment enrollment = _mapper.Map<EnrollmentDTO, Enrollment>(enrollmentDTO);
            bool canParticipantAttendEvent = await CanParticipantAttendEvent(enrollment.ParticipantId, enrollment.EventId, cancellationToken);
            if (canParticipantAttendEvent)
            {
                throw new ArgumentException("Вы уже зарегистрированы на это событие.");
            }
            var validationResult = await _enrollmentValidator.ValidateAsync(enrollment, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                throw new ValidationException(string.Join(Environment.NewLine, errors));
            }
            return await _enrollmentRepository.CreateUpdateEnrollment(_mapper.Map<Enrollment, EnrollmentDTO>(enrollment), cancellationToken);
        }

        public async Task<bool> CanParticipantAttendEvent(int participantId, int eventId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var existingEnrollments = await _enrollmentRepository.GetEnrollmentsForParticipantOnDate(eventId, participantId, cancellationToken);
            return existingEnrollments.Any();
        }

        public async Task<bool> DeleteEnrollment(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _enrollmentRepository.DeleteEnrollment(id, cancellationToken);
        }

        public async Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _enrollmentRepository.GetEnrollments(eventId, cancellationToken);
        }

        public async Task<EnrollmentDTO> GetEnrollmentById(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _enrollmentRepository.GetEnrollmentById(id, cancellationToken);
        }
    }
}