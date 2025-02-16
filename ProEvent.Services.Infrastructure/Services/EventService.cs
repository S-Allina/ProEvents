using Microsoft.Extensions.Caching.Memory;
using ProEvent.Services.Core;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;
using AutoMapper;
using FluentValidation;
using ProEvent.Services.Core.Interfaces.IRepository;
using ProEvent.Services.Core.Interfaces.IService;

namespace ProEvent.Services.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IValidator<Event> _eventValidator;
        private const string EventImageCacheKeyPrefix = "EventImage_";

        public EventService(IEventRepository eventRepository, IMemoryCache cache, IMapper mapper, IValidator<Event> eventValidator)
        {
            _eventRepository = eventRepository;
            _cache = cache;
            _mapper = mapper;
            _eventValidator = eventValidator;
        }

        public async Task<EventDTO> GetEventById(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var eventItem = await _eventRepository.GetEventById(id, cancellationToken);
            if (eventItem == null)
            {
                return null;
            }

            eventItem.Image = await GetEventImage(id, eventItem.Image);

            EventStatus status = await _eventRepository.CalculateEventStatus(eventItem, cancellationToken);
            EventDTO eventDto = _mapper.Map<EventDTO>(eventItem);
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
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _eventRepository.GetEvents(pageNumber, pageSize, startDate, endDate, location, category, name, cancellationToken);
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

        public async Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Event eventEntity = _mapper.Map<Event>(eventDTO);

            var validationResult = await _eventValidator.ValidateAsync(eventEntity, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ProEvent.Services.Core.Exceptions.ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            if (eventEntity.Id == 0)
            {
                await _eventRepository.CreateUpdateEvent(eventDTO, cancellationToken);
            }
            else
            {
                await _eventRepository.CreateUpdateEvent(eventDTO, cancellationToken);
            }

            return _mapper.Map<EventDTO>(eventEntity);
        }
    }
}