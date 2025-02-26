using ProEvent.Domain.Models;

namespace ProEvent.DAL.Interfaces.IRepository
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<Participant>> GetParticipants(CancellationToken cancellationToken);
        Task<Participant> GetParticipantByUserId(string id, CancellationToken cancellationToken);
        Task<Participant> CreateUpdateParticipant(Participant participant, CancellationToken cancellationToken);
        Task<bool> DeleteParticipant(int id, CancellationToken cancellationToken);
    }
}
