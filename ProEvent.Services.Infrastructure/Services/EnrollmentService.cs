using AutoMapper;
using FluentValidation;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
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
        public async Task<List<ParticipantWithRegistrationDTO>> GetEnrollmentsWithParticipantInfo(int? eventId = null)
        {
            IEnumerable<EnrollmentDTO> enrollments = await _enrollmentRepository.GetEnrollments(eventId);

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
        public async Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO)
        {
            int? participantId = await _enrollmentRepository.GetParticipantIdByUserId(enrollmentDTO.UserId);
            if (participantId == null)
            {
                throw new ArgumentException("Участник не найден");
            }
            enrollmentDTO.ParticipantId = participantId.Value;

            Enrollment enrollment = _mapper.Map<EnrollmentDTO, Enrollment>(enrollmentDTO);
            bool canParticipantAttendEvent =await CanParticipantAttendEvent(enrollment.ParticipantId, enrollment.EventId);
            if (canParticipantAttendEvent)
            {
                throw new ArgumentException("Вы уже зарегистрированы на это событие.");
            }
            var validationResult = await _enrollmentValidator.ValidateAsync(enrollment);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                throw new ValidationException(string.Join(Environment.NewLine, errors));
            }
            return await _enrollmentRepository.CreateUpdateEnrollment(_mapper.Map<Enrollment, EnrollmentDTO>(enrollment));
        }
    
    public async Task<bool> CanParticipantAttendEvent(int participantId, int eventId)
        {
            var existingEnrollments = await _enrollmentRepository.GetEnrollmentsForParticipantOnDate(eventId, participantId);
            return !existingEnrollments.Any();
        }
        public async Task<bool> DeleteEnrollment(int id)
        {
            return await _enrollmentRepository.DeleteEnrollment(id);
        }
        public async Task<IEnumerable<EnrollmentDTO>> GetEnrollments(int? eventId = null)
        {

            return await _enrollmentRepository.GetEnrollments(eventId);
        }

        public async Task<EnrollmentDTO> GetEnrollmentById(int id)
        {
            return await _enrollmentRepository.GetEnrollmentById(id);
        }
    }
}