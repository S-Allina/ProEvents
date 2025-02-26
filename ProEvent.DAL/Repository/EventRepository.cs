
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProEvent.DAL.Data;
using ProEvent.DAL.Interfaces.IRepository;
using ProEvent.Domain.Models;

namespace ProEvent.DAL.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<Event> _eventValidator;
        private readonly IMemoryCache _cache;
        private const string EventImageCacheKeyPrefix = "EventImage_";

        public EventRepository(ApplicationDbContext db,  IValidator<Event> eventValidator, IMemoryCache cache)
        {
            _db = db;
            _eventValidator = eventValidator;
            _cache = cache;
        }

        public async Task<Event> CreateUpdateEvent(Event events, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Event existingEvent = null;
            if (events.Id != 0)
            {
                existingEvent = await _db.Events.FirstOrDefaultAsync(e=>e.Id==events.Id);
                if (existingEvent == null)
                {
                    throw new ArgumentException($"Event with ID {events.Id} not found.");
                }

                if (!string.IsNullOrEmpty(events.Name)) { existingEvent.Name = events.Name; }
                if (!string.IsNullOrEmpty(events.Description)) { existingEvent.Description = events.Description; }
                if (!string.IsNullOrEmpty(events.Location)) { existingEvent.Location = events.Location; }
                if (!string.IsNullOrEmpty(events.Category)) { existingEvent.Category = events.Category; }
                if (events.Date != default(DateTime)) { existingEvent.Date = events.Date; }
                if (events.Image != null) { existingEvent.Image = events.Image; }
                if (events.MaxParticipants != 0) { existingEvent.MaxParticipants = events.MaxParticipants; }
            }
            else
            {
                var validationResult = await _eventValidator.ValidateAsync(events, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                    throw new ValidationException(string.Join(Environment.NewLine, errors));
                }
                _db.Events.Add(events);
            }

            await _db.SaveChangesAsync(cancellationToken);

            string cacheKey = $"{EventImageCacheKeyPrefix}{events.Id}";
            _cache.Remove(cacheKey);

            return existingEvent;
        }

        public async Task<bool> DeleteEvent(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                Event events = await _db.Events.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
                if (events == null)
                {
                    return false;
                }

                _db.Events.Remove(events);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Event> GetEventById(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Event events = await _db.Events.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (events == null) { return null; }
            return events;
        }

        public async Task<(IEnumerable<Event> Events, int TotalCount)> GetEvents(
           int pageNumber = 1,
           int pageSize = 4,
           DateTime? startDate = null,
           DateTime? endDate = null,
           string? location = null,
           string? category = null,
           string? name = null,
           bool isPassed = false,
           CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<Event> query = _db.Events;

            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.Location.Contains(location));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category.Contains(category));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name));
            }
            if (!isPassed) query = query.Where(e => e.Date >= DateTime.Now);
            int totalCount =  query.Count();

            List<Event> events =  query.OrderBy(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

          
            return (events, totalCount);
        }
        public async Task<IEnumerable<EventWithRegistration>> GetEventsByUser(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var eventRegistrations = await _db.Events.Join(
                    _db.Enrollments,
                    e => e.Id,
                    en => en.EventId,
                    (e, en) => new { Event = e, Enrollment = en })
                .Join(
                    _db.Participants,
                    een => een.Enrollment.ParticipantId,
                    p => p.Id,
                    (een, p) => new { Event = een.Event, Participant = p, Enrollment = een.Enrollment })
                .Where(ep => ep.Participant.UserId == userId)
                .Select(ep => new EventWithRegistration
                {
                    ParticipanId = ep.Participant.Id,
                    EnrollmentId = ep.Enrollment.Id,
                    EventId = ep.Event.Id,
                    Name = ep.Event.Name,
                    Description = ep.Event.Description,
                    Date = ep.Event.Date,
                    Location = ep.Event.Location,
                    Category = ep.Event.Category,
                    MaxParticipants = ep.Event.MaxParticipants,
                    RegistrationDate = ep.Enrollment.RegistrationDate
                }).OrderBy(e => e.Date)
                .ToListAsync(cancellationToken);

            
            return eventRegistrations;
        }
       
    }
}

