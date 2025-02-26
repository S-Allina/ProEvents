using Microsoft.EntityFrameworkCore;
using ProEvent.DAL.Data;
using ProEvent.DAL.Interfaces.IRepository;
using ProEvent.Domain.Models;

namespace ProEvent.DAL.Repository
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _db;

        public ParticipantRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Participant> CreateUpdateParticipant(Participant participant, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (participant.Id != 0)
            {
                var existingParticipant = await _db.Participants.FindAsync(new object[] { participant.Id }, cancellationToken);
                if (existingParticipant == null)
                {
                    throw new ArgumentException($"Participant with ID {participant.Id} not found.");
                }
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

            return participant;
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

        public async Task<Participant> GetParticipantByUserId(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Participant participant = await _db.Participants
                .Where(x => x.UserId == id)
                .FirstOrDefaultAsync(cancellationToken);

            return participant;
        }

        public async Task<IEnumerable<Participant>> GetParticipants(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<Participant> participants = await _db.Participants.ToListAsync(cancellationToken);
            return participants;
        }
    }
}
