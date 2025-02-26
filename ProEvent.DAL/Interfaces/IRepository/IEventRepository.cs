using ProEvent.Domain.Models;

namespace ProEvent.DAL.Interfaces.IRepository
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventWithRegistration>> GetEventsByUser(string UserId, CancellationToken cancellationToken);
        Task<Event> GetEventById(int id, CancellationToken cancellationToken);
        Task<(IEnumerable<Event> Events, int TotalCount)> GetEvents(
               int pageNumber = 1,
               int pageSize = 4,
               DateTime? startDate = null,
               DateTime? endDate = null,
               string? location = null,
               string? category = null,
               string? name = null,
               bool isPassed=false,
               CancellationToken cancellationToken = default);
        Task<Event> CreateUpdateEvent(Event events, CancellationToken cancellationToken);
        Task<bool> DeleteEvent(int id, CancellationToken cancellationToken);
    }
}
