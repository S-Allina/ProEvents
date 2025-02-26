
using ProEvent.BLL.DTOs;

namespace ProEvent.BLL.Interfaces.IService
{
    public interface IEventService
    {
        Task<EventDTO> GetEventById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<EventWithRegistrationDTO>> GetEventByUserId(string userId, CancellationToken cancellationToken);
        Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO, CancellationToken cancellationToken);
        Task<bool> DeleteEvent(int id, CancellationToken cancellationToken);
        Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(
            int pageNumber = 1,
            int pageSize = 4,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? location = null,
            string? category = null,
            string? name = null,
            bool isPassed = false,
            CancellationToken cancellationToken = default);
    }
}
