
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
using ProEvent.Services.Core.Validators;
using ProEvent.Services.Infrastructure.Data;

namespace ProEvent.Services.Infrastructure.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IValidator<Event> _eventValidator;
        private const string EventImageCacheKeyPrefix = "EventImage_";

        public EventRepository(ApplicationDbContext db, IMapper mapper, IValidator<Event> eventValidator, IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _cache = cache;
            _eventValidator = eventValidator;
        }


        public async Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO)
        {
            Event existingEvent = null;

            if (eventDTO.Id != 0)
            {
                existingEvent = await _db.Events.FindAsync(eventDTO.Id);

                if (existingEvent == null)
                {
                    throw new ArgumentException($"Event with ID {eventDTO.Id} not found.");
                }
                if (!string.IsNullOrEmpty(eventDTO.Name))
                {
                    existingEvent.Name = eventDTO.Name;
                }
                if (!string.IsNullOrEmpty(eventDTO.Description))
                {
                    existingEvent.Description = eventDTO.Description;
                }
                if (!string.IsNullOrEmpty(eventDTO.Location))
                {
                    existingEvent.Location = eventDTO.Location;
                }
                if (!string.IsNullOrEmpty(eventDTO.Category))
                {
                    existingEvent.Category = eventDTO.Category;
                }
                if (eventDTO.Date != default(DateTime))
                {
                    existingEvent.Date = eventDTO.Date;
                }

                if (eventDTO.Image != null)
                {
                    existingEvent.Image = eventDTO.Image;
                }

                if (eventDTO.MaxParticipants != 0)
                {
                    existingEvent.MaxParticipants = eventDTO.MaxParticipants;
                }
            }
            else
            {
                existingEvent = _mapper.Map<EventDTO, Event>(eventDTO);

                var validationResult = await _eventValidator.ValidateAsync(existingEvent);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                    throw new ValidationException(string.Join(Environment.NewLine, errors));
                }
                _db.Events.Add(existingEvent);
            }

            await _db.SaveChangesAsync();

            string cacheKey = $"{EventImageCacheKeyPrefix}{existingEvent.Id}";
            _cache.Remove(cacheKey);

            return _mapper.Map<Event, EventDTO>(existingEvent);
        }
        public async Task<bool> DeleteEvent(int id)
        {
            try
            {

                Event events = await _db.Events.FirstOrDefaultAsync(u => u.Id == id);
                if (events == null)
                {
                    return false;
                }
                _db.Events.Remove(events);
                await _db.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public async Task<Event> GetEventById(int id)
        {
            Event events = await _db.Events.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (events == null) { return null; }



            return events;
        }
        public async Task<IEnumerable<EventDTO>> GetEventByName(string name)
        {
            List<Event> events = await _db.Events.Where(e => e.Name.Contains(name)).ToListAsync();


            List<EventDTO> eventDtos = _mapper.Map<List<EventDTO>>(events);

            for (int i = 0; i < eventDtos.Count; i++)
            {
                var eventDto = eventDtos[i];
                var eventItem = events[i]; // Get the corresponding Event object

                int enrollmentCount = await _db.Enrollments.CountAsync(e => e.EventId == eventDto.Id);
                eventDto.Status = await CalculateEventStatus(eventItem);

                if (eventItem.Image != null)
                {
                    int eventId = eventDto.Id;
                    string cacheKey = $"{EventImageCacheKeyPrefix}{eventId}";

                    eventDto.Image = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        return eventItem.Image;
                    });
                }
            }

            return eventDtos.Where(e => e.Status != EventStatus.Passed);

        }
        public async Task<IEnumerable<EventDTO>> GetFilteredEvents(DateTime? startDate, DateTime? endDate, string location, string category)
        {
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
            var events = await query.ToListAsync();

            List<EventDTO> eventDtos = _mapper.Map<List<EventDTO>>(events);

            for (int i = 0; i < eventDtos.Count; i++)
            {
                var eventDto = eventDtos[i];
                var eventItem = events[i];

                int enrollmentCount = await _db.Enrollments.CountAsync(e => e.EventId == eventDto.Id);
                eventDto.Status = await CalculateEventStatus(eventItem);

                if (eventItem.Image != null)
                {
                    int eventId = eventDto.Id;
                    string cacheKey = $"{EventImageCacheKeyPrefix}{eventId}";

                    eventDto.Image = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        return eventItem.Image;
                    });
                }
            }
            if (startDate.HasValue || endDate.HasValue)
            {
                return eventDtos;
            }
            else
            {
                return eventDtos.Where(e => e.Status != EventStatus.Passed);
            }

        }
        public async Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(int pageNumber = 1, int pageSize = 4)
        {
            // 1. Get all events (for status calculation and filtering)
            List<Event> allEvents = await _db.Events.ToListAsync();

            // 2. Calculate Event Status and Filter
            List<EventDTO> eventDtos = new List<EventDTO>();
            foreach (var eventItem in allEvents)
            {
                EventStatus status = await CalculateEventStatus(eventItem);

                if (status != EventStatus.Passed) // Filter out Passed events
                {
                    EventDTO eventDto = _mapper.Map<EventDTO>(eventItem);
                    eventDto.Status = status;

                    if (eventItem.Image != null)
                    {
                        int eventId = eventDto.Id;
                        string cacheKey = $"{EventImageCacheKeyPrefix}{eventId}";

                        eventDto.Image = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                        {
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                            return eventItem.Image;
                        });
                    }
                    eventDtos.Add(eventDto);
                }
            }

            // 3. Apply Pagination after filtering
            var pagedEvents = eventDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedEvents, eventDtos.Count); // Return paged events and total count after filtering
        }
        public async Task<EventStatus> CalculateEventStatus(Event events)
        {
            int enrollmentCount = await _db.Enrollments.CountAsync(e => e.EventId == events.Id);

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
        public async Task<IEnumerable<EventWithRegistrationDTO>> GetEventsByUser(string userId)
        {
            var eventRegistrations = await _db.Events
                .Join(
                    _db.Enrollments,
                    e => e.Id,
                    en => en.EventId,
                    (e, en) => new { Event = e, Enrollment = en }
                )
                .Join(
                    _db.Participants,
                    een => een.Enrollment.ParticipantId,
                    p => p.Id,
                    (een, p) => new
                    {
                        Event = een.Event,
                        Participant = p,
                        Enrollment = een.Enrollment
                    }
                )
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
                })
                .ToListAsync();

            foreach (var registrationDto in eventRegistrations)
            {
                var theEvent = await _db.Events.FirstOrDefaultAsync(eventItem => eventItem.Name == registrationDto.Name);
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
                    int enrollmentCount = await _db.Enrollments.CountAsync(e => e.EventId == theEvent.Id);
                    registrationDto.Status = await CalculateEventStatus(theEvent);
                }
                else
                {
                    registrationDto.Status = EventStatus.Passed;
                }
            }

            return eventRegistrations;
        }

    }

}