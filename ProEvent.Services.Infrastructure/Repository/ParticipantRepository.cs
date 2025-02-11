
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
using ProEvent.Services.Infrastructure.Data;

namespace ProEvent.Services.Infrastructure.Repository
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        private readonly IValidator<Participant> _participantValidator;
        public ParticipantRepository(ApplicationDbContext db, IMapper mapper, IValidator<Participant> participantValidator) 
        {
            _db = db;
            _mapper = mapper;
            _participantValidator = participantValidator;
        }


        public async Task<ParticipantDTO> CreateUpdateParticipant(ParticipantDTO participantDTO)
        {
            Participant participants = _mapper.Map<ParticipantDTO, Participant>(participantDTO);
            var validationResult = await _participantValidator.ValidateAsync(participants);

    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
        throw new ValidationException(string.Join(Environment.NewLine, errors));
    }

    Participant participant = _mapper.Map<ParticipantDTO, Participant>(participantDTO);

    //2. Valdiate Entity
    var entityValidationResult = await _participantValidator.ValidateAsync(participant);
    if (!entityValidationResult.IsValid)
    {
        var errors = entityValidationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
        throw new ValidationException(string.Join(Environment.NewLine, errors));
    }

    if (participant.Id != 0)
    {
        // Ensure the participant exists before attempting to update

        var existingParticipant = await _db.Participants.FindAsync(participant.Id);
        if (existingParticipant == null)
        {
            throw new ArgumentException($"Participant with ID {participant.Id} not found.");
        }
        _db.Participants.Update(participant);
    }
    else
    {

        _db.Participants.Add(participant);
    }

    await _db.SaveChangesAsync();

    return _mapper.Map<Participant, ParticipantDTO>(participant);
}

        public async Task<bool> DeleteParticipant(int id)
        {
            try
            {

                Participant participants = await _db.Participants.FirstOrDefaultAsync(u => u.Id == id);
                if (participants == null)
                {
                    return false;
                }
                _db.Participants.Remove(participants);
                await _db.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public async Task<ParticipantDTO> GetParticipantByUserId(string id)
        {
            Participant participants = await _db.Participants.Where(x => x.UserId == id).FirstOrDefaultAsync();
            return _mapper.Map<ParticipantDTO>(participants);
        }

        public async Task<IEnumerable<ParticipantDTO>> GetParticipants()
        {
            List<Participant> participants = await _db.Participants.ToListAsync();
            return _mapper.Map<List<ParticipantDTO>>(participants);
        }
}}
