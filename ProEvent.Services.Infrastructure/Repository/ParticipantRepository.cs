
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces.IRepository;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Infrastructure.Data;

namespace ProEvent.Services.Infrastructure.Repository
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ParticipantRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ParticipantDTO> CreateUpdateParticipant(ParticipantDTO participantDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Participant participant = _mapper.Map<ParticipantDTO, Participant>(participantDTO);

            if (participant.Id != 0)
            {
                var existingParticipant = await _db.Participants.FindAsync(new object[] { participant.Id }, cancellationToken);
                if (existingParticipant == null)
                {
                    throw new ArgumentException($"Participant with ID {participant.Id} not found.");
                }
                _mapper.Map(participantDTO, existingParticipant);
                _db.Entry(existingParticipant).State = EntityState.Modified;
            }
            else
            {
                _db.Participants.Add(participant);
            }

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ошибка при сохранении участника в базу данных.", ex);
            }

            return _mapper.Map<Participant, ParticipantDTO>(participant);
        }

        public async Task<bool> DeleteParticipant(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Participant participant = await _db.Participants.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
                if (participant == null)
                {
                    return false;
                }

                _db.Participants.Remove(participant);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ParticipantDTO> GetParticipantByUserId(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Participant participant = await _db.Participants
                .Where(x => x.UserId == id)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<ParticipantDTO>(participant);
        }

        public async Task<IEnumerable<ParticipantDTO>> GetParticipants(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<Participant> participants = await _db.Participants.ToListAsync(cancellationToken);
            return _mapper.Map<List<ParticipantDTO>>(participants);
        }
    }
}
