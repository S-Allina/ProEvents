

using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using ProEvent.BLL.DTOs;
using ProEvent.BLL.Interfaces.IService;
using ProEvent.BLL.Interfaces.IService;
using ProEvent.DAL.Interfaces.IRepository;
using ProEvent.Domain.Enums;
using ProEvent.Domain.Models;
using System.Xml.Linq;

namespace ProEvent.BLL.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IValidator<Event> _eventValidator;
        private const string EventImageCacheKeyPrefix = "EventImage_";

        public EventService(IEventRepository eventRepository, IMemoryCache cache, IMapper mapper, IValidator<Event> eventValidator, IEnrollmentService enrollmentService)
        {
            _eventRepository = eventRepository;
            _cache = cache;
            _mapper = mapper;
            _eventValidator = eventValidator;
            _enrollmentService = enrollmentService;
        }

        public async Task<EventDTO> GetEventById(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var eventItem = await _eventRepository.GetEventById(id, cancellationToken);
            if (eventItem == null)
            {
                return null;
            }

            eventItem.Image = await GetEventImage(id, eventItem.Image);
            EventDTO eventDto = _mapper.Map<EventDTO>(eventItem);
            EventStatus status = await CalculateEventStatus(eventDto, cancellationToken);
            eventDto.Status = status;
            return eventDto;
        }

        public async Task<(IEnumerable<EventDTO> Events, int TotalCount)> GetEvents(
            int pageNumber = 1,
            int pageSize = 4,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? location = null,
            string? category = null,
            string? name = null,
            bool isPassed= false,
            CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested();
           var events = await _eventRepository.GetEvents(pageNumber, pageSize, startDate, endDate, location, category, name,isPassed, cancellationToken);
           var eventDTOs= _mapper.Map<IEnumerable<Event>, List<EventDTO>>(events.Events);
            foreach (var eventDTO in eventDTOs)
            {
                if (eventDTO.Image != null)
                {
                    int eventId = eventDTO.Id;
                    string cacheKey = $"{EventImageCacheKeyPrefix}{eventId}";
                    eventDTO.Image = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        return eventDTO.Image;
                    });
                }
                if (eventDTO != null)
                {
                    eventDTO.Status = await CalculateEventStatus(eventDTO, cancellationToken);
                }
                if(!isPassed) eventDTOs=eventDTOs.Where(e=>e.Status!= EventStatus.Passed).ToList();
            }
            return (eventDTOs, events.TotalCount);
        }

        private async Task<byte[]> GetEventImage(int eventId, byte[] imageBytes)
        {
            if (imageBytes == null)
            {
                return null;
            }

            string cacheKey = $"{EventImageCacheKeyPrefix}{eventId}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return imageBytes;
            });
        }

        public async Task<bool> DeleteEvent(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await _eventRepository.DeleteEvent(id, cancellationToken);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<EventWithRegistrationDTO>> GetEventByUserId (string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var eventRegistrations = await _eventRepository.GetEventsByUser(userId, cancellationToken);
            var eventRegistrationsDTOs = _mapper.Map<IEnumerable<EventWithRegistration>, List<EventWithRegistrationDTO>>(eventRegistrations);
            foreach (var registrationDto in eventRegistrationsDTOs)
            {
                var events = _eventRepository.GetEventById(registrationDto.EventId, cancellationToken);

                if (registrationDto != null)
                {
                    var eventDTO = _mapper.Map<EventDTO>(events);
                    registrationDto.Status = await CalculateEventStatus(eventDTO, cancellationToken);
                }
                else
                {
                    registrationDto.Status = EventStatus.Passed;
                }
            }
            return eventRegistrationsDTOs;
        }

        public async Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Event eventEntity = _mapper.Map<Event>(eventDTO);

            var validationResult = await _eventValidator.ValidateAsync(eventEntity, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ProEvent.Domain.Exceptions.ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            if (eventEntity.Id == 0)
            {
                await _eventRepository.CreateUpdateEvent(eventEntity, cancellationToken);
            }
            else
            {
                await _eventRepository.CreateUpdateEvent(eventEntity, cancellationToken);
            }

            return _mapper.Map<EventDTO>(eventEntity);
        }

        public async Task<EventStatus> CalculateEventStatus(EventDTO eventDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var enrollment = await _enrollmentService.GetEnrollments(eventDTO.Id, cancellationToken);
            int enrollmentCount = enrollment.Count();
            if (eventDTO.Date < DateTime.Now)
            {
                return EventStatus.Passed;
            }
            else if (enrollmentCount >= eventDTO.MaxParticipants)
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