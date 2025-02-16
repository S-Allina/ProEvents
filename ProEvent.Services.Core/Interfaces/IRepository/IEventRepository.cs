using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Interfaces.IRepository
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventWithRegistrationDTO>> GetEventsByUser(string UserId, CancellationToken cancellationToken);
        Task<Event> GetEventById(int id, CancellationToken cancellationToken);
        Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(
               int pageNumber = 1,
               int pageSize = 4,
               DateTime? startDate = null,
               DateTime? endDate = null,
               string? location = null,
               string? category = null,
               string? name = null,
               CancellationToken cancellationToken = default);
        Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO, CancellationToken cancellationToken);
        Task<bool> DeleteEvent(int id, CancellationToken cancellationToken);
        Task<EventStatus> CalculateEventStatus(Event events, CancellationToken cancellationToken);
    }
}
