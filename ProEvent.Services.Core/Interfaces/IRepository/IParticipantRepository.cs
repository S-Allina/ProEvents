using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Interfaces.IRepository
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<ParticipantDTO>> GetParticipants(CancellationToken cancellationToken);
        Task<ParticipantDTO> GetParticipantByUserId(string id, CancellationToken cancellationToken);
        Task<ParticipantDTO> CreateUpdateParticipant(ParticipantDTO participantDTO, CancellationToken cancellationToken);
        Task<bool> DeleteParticipant(int id, CancellationToken cancellationToken);
    }
}
