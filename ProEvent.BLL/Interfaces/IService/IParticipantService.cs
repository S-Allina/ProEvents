using ProEvent.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.BLL.Interfaces.IService
{
    public interface IParticipantService
    {
        Task<IEnumerable<ParticipantDTO>> GetParticipants(CancellationToken cancellationToken);
        Task<ParticipantDTO> GetParticipantByUserId(string userId, CancellationToken cancellationToken);
        Task<ParticipantDTO> CreateUpdateParticipant(ParticipantDTO participantDTO, CancellationToken cancellationToken);
        Task<bool> DeleteParticipant(int id, CancellationToken cancellationToken);
    }
}
