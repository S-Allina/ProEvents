
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Repository
{
    public interface IEventRepository
    {
       
        Task<IEnumerable<EventWithRegistrationDTO>> GetEventsByUser(string UserId);
        Task<Event> GetEventById(int id);
        Task<IEnumerable<EventDTO>> GetEventByName(string name);
        Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(int pageNumber = 1, int pageSize = 10);
        Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO);
        Task<bool> DeleteEvent(int id);
        Task<IEnumerable<EventDTO>> GetFilteredEvents(DateTime? startDate, DateTime? endDate, string location, string category);
        Task<EventStatus> CalculateEventStatus(Event events);
    }
}
