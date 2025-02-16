using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Interfaces.IRepository;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Validators;
using ProEvent.Services.Infrastructure.Data;

namespace ProEvent.Services.Infrastructure.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IValidator<Event> _eventValidator;
        private readonly IMemoryCache _cache;
        private const string EventImageCacheKeyPrefix = "EventImage_";

        public EventRepository(ApplicationDbContext db, IMapper mapper, IValidator<Event> eventValidator, IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _eventValidator = eventValidator;
            _cache = cache;
        }

        public async Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Event existingEvent = null;
            if (eventDTO.Id != 0)
            {
                existingEvent = await _db.Events.FindAsync(new object[] { eventDTO.Id }, cancellationToken);
                if (existingEvent == null)
                {
                    throw new ArgumentException($"Event with ID {eventDTO.Id} not found.");
                }

                if (!string.IsNullOrEmpty(eventDTO.Name)) { existingEvent.Name = eventDTO.Name; }
                if (!string.IsNullOrEmpty(eventDTO.Description)) { existingEvent.Description = eventDTO.Description; }
                if (!string.IsNullOrEmpty(eventDTO.Location)) { existingEvent.Location = eventDTO.Location; }
                if (!string.IsNullOrEmpty(eventDTO.Category)) { existingEvent.Category = eventDTO.Category; }
                if (eventDTO.Date != default(DateTime)) { existingEvent.Date = eventDTO.Date; }
                if (eventDTO.Image != null) { existingEvent.Image = eventDTO.Image; }
                if (eventDTO.MaxParticipants != 0) { existingEvent.MaxParticipants = eventDTO.MaxParticipants; }
            }
            else
            {
                existingEvent = _mapper.Map<EventDTO, Event>(eventDTO);
                var validationResult = await _eventValidator.ValidateAsync(existingEvent, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                    throw new ValidationException(string.Join(Environment.NewLine, errors));
                }
                _db.Events.Add(existingEvent);
            }

            await _db.SaveChangesAsync(cancellationToken);

            string cacheKey = $"{EventImageCacheKeyPrefix}{existingEvent.Id}";
            _cache.Remove(cacheKey);

            return _mapper.Map<Event, EventDTO>(existingEvent);
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

        public async Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(
           int pageNumber = 1,
           int pageSize = 4,
           DateTime? startDate = null,
           DateTime? endDate = null,
           string? location = null,
           string? category = null,
           string? name = null,
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

            int totalCount = await query.CountAsync(cancellationToken);

            List<Event> events = await query.OrderBy(e=>e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var eventDTOs = _mapper.Map<List<EventDTO>>(events);

            return (eventDTOs, totalCount);
        }
        public async Task<IEnumerable<EventWithRegistrationDTO>> GetEventsByUser(string userId, CancellationToken cancellationToken)
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
                .Select(ep => new EventWithRegistrationDTO
                {
                    ParticipanId = ep.Participant.Id,
                    EnrollmentId = ep.Enrollment.Id,
                    Name = ep.Event.Name,
                    Description = ep.Event.Description,
                    Date = ep.Event.Date,
                    Location = ep.Event.Location,
                    Category = ep.Event.Category,
                    MaxParticipants = ep.Event.MaxParticipants,
                    RegistrationDate = ep.Enrollment.RegistrationDate
                }).OrderBy(e => e.Date)
                .ToListAsync(cancellationToken);

            foreach (var registrationDto in eventRegistrations)
            {
                var theEvent = await _db.Events.FirstOrDefaultAsync(eventItem => eventItem.Name == registrationDto.Name, cancellationToken);
                if (theEvent.Image != null)
                {
                    int eventId = theEvent.Id;
                    string cacheKey = $"{EventImageCacheKeyPrefix}{eventId}";
                    theEvent.Image = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        return theEvent.Image;
                    });
                }
                if (theEvent != null)
                {
                    registrationDto.Status = await CalculateEventStatus(theEvent, cancellationToken);
                }
                else
                {
                    registrationDto.Status = EventStatus.Passed;
                }
            }
            return eventRegistrations;
        }
        public async Task<EventStatus> CalculateEventStatus(Event events, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int enrollmentCount = await _db.Enrollments.CountAsync(e => e.EventId == events.Id, cancellationToken);
            if (events.Date < DateTime.Now)
            {
                return EventStatus.Passed;
            }
            else if (enrollmentCount >= events.MaxParticipants)
            {
                return EventStatus.NoPlaces;
            }
            else
            {
                return EventStatus.Relevant;
            }
        }
    }
}

