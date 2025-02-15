using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Interfaces
{
    public interface IEventService
    {
        Task<EventDTO> GetEventById(int id);
        Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO);
        Task<bool> DeleteEvent(int id);
        Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(
       int pageNumber = 1,
       int pageSize = 4,
       DateTime? startDate = null,
       DateTime? endDate = null,
       string? location = null,
       string? category = null,
       string? name = null);

    }
}
