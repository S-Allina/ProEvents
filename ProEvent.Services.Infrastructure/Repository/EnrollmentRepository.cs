
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
using ProEvent.Services.Infrastructure.Data;

namespace ProEvent.Services.Infrastructure.Repository
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        private readonly IValidator<Enrollment> _enrollmentValidator;
        public EnrollmentRepository(ApplicationDbContext db, IMapper mapper, IValidator<Enrollment> enrollmentValidator) 
        {
            _db = db;
            _mapper = mapper;
            _enrollmentValidator = enrollmentValidator;
        }

        // EnrollmentRepository.cs
        public async Task<IEnumerable<ParticipantDTO>> GetParticipantsByEventId(int eventId)
        {
            var enrollments = await _db.Enrollments
                .Where(e => e.EventId == eventId)
                .Include(e => e.Participant) // Загружаем информацию об участнике
                .ToListAsync();

            // Маппим Enrollment в ParticipantDTO
            return _mapper.Map<List<ParticipantDTO>>(enrollments.Select(e => e.Participant).ToList());
        }
    
       
        
        
        public async Task<int?> GetParticipantIdByUserId(string userId)
        {
            return await _db.Participants.FirstOrDefaultAsync(p => p.UserId == userId).ContinueWith(t => t.Result?.Id);
        }



        public async Task<EnrollmentDTO> CreateUpdateEnrollment(EnrollmentDTO enrollmentDTO)
        {
            // 1. Validate the EnrollmentDTO
         

            //enrollmentDTO.ParticipantId = _db.Participants.FirstOrDefault(p => p.UserId == enrollmentDTO.UserId).Id; //Удаляем отсюда!!!  (Это *должно* быть в логике сервиса, если это нужно. Но лучше пересмотреть подход.)

            Enrollment enrollment = _mapper.Map<EnrollmentDTO, Enrollment>(enrollmentDTO);
            var validationResult = await _enrollmentValidator.ValidateAsync(enrollment);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                throw new ValidationException(string.Join(Environment.NewLine, errors));
            }
            if (enrollment.Id != 0)
            {
                // Валидация перед обновлением существующей записи
                var existingEnrollment = await _db.Enrollments.FindAsync(enrollment.Id);
                if (existingEnrollment == null)
                {
                    throw new ArgumentException($"Enrollment with ID {enrollment.Id} not found.");
                }

                // Validate existing enrollment
                var entityValidationResult = await _enrollmentValidator.ValidateAsync(enrollment);

                if (!entityValidationResult.IsValid)
                {
                    var errors = entityValidationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                    throw new ValidationException($"Enrollment Entity Validation failed:{string.Join(Environment.NewLine, errors)}");
                }

                _db.Enrollments.Update(enrollment);
            }
            else
            {
                // Validate new enrollment
                var entityValidationResult = await _enrollmentValidator.ValidateAsync(enrollment);

                if (!entityValidationResult.IsValid)
                {
                    var errors = entityValidationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                    throw new ValidationException($"Enrollment Entity Validation failed:{string.Join(Environment.NewLine, errors)}");
                }
                _db.Enrollments.Add(enrollment);
            }

            await _db.SaveChangesAsync();

            return _mapper.Map<Enrollment, EnrollmentDTO>(enrollment);
        }
        public async Task<bool> DeleteEnrollment(int id)
        {
            try
            {

                Enrollment enrollment = await _db.Enrollments.FirstOrDefaultAsync(u => u.Id == id);
                if (enrollment == null)
                {
                    return false;
                }
                _db.Enrollments.Remove(enrollment);
                await _db.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public async Task<EnrollmentDTO> GetEnrollmentById(int id)
        {
            Enrollment enrollment = await _db.Enrollments.Where(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<EnrollmentDTO>(enrollment);
        }
        public async Task<IEnumerable<EnrollmentDTO>> GetEnrollmentsByEventId(int eventId)
        {
            List<Enrollment> enrollment = await _db.Enrollments
                .Include(e => e.Participant) // Важно включить данные Participant
                .Where(e => e.EventId == eventId)
                .ToListAsync();
            return _mapper.Map<List<EnrollmentDTO>>(enrollment);
        }
        public async Task<IEnumerable<EnrollmentDTO>> GetEnrollments()
        {
            List<Enrollment> enrollment = await _db.Enrollments.ToListAsync();
            return _mapper.Map<List<EnrollmentDTO>>(enrollment);
        }
        public async Task<List<Enrollment>> GetEnrollmentsForParticipantOnDate(int eventId, int participantId, DateTime eventDate)
        {
            return await _db.Enrollments
                .Include(e => e.Event) // Подгружаем информацию о событии
                .Where(e =>
                    e.ParticipantId == participantId && // Фильтруем по участнику
                    (e.EventId == eventId || (e.EventId != eventId && // Исключаем текущее событие
                    e.Event.Date == eventDate))) // Фильтруем по дате
                .ToListAsync();
        }

    }
}
