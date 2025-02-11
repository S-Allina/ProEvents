
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Repository
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<ParticipantDTO>> GetParticipants();
        Task<ParticipantDTO> GetParticipantByUserId(string id);
        Task<ParticipantDTO> CreateUpdateParticipant(ParticipantDTO participantDTO);
        Task<bool> DeleteParticipant(int id);
    }
}
