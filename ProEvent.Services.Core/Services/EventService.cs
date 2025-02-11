using Microsoft.Extensions.Caching.Memory;
using ProEvent.Services.Core;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
using AutoMapper;

namespace ProEvent.Services.Core.Services
{
    public class EventService : IEventService
    {

        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private const string EventImageCacheKeyPrefix = "EventImage_";

        public EventService(IEventRepository eventRepository, IMemoryCache cache, IMapper mapper, IEnrollmentRepository enrollmentRepository)
        {
            _eventRepository = eventRepository;
            _cache = cache;
            _mapper = mapper;
            _enrollmentRepository = enrollmentRepository;

        }

        public async Task<bool> CanParticipantAttendEvent(int participantId, DateTime eventDate, int eventId)
        {
            // Получаем список записей участника на другие события, которые пересекаются по дате.
            var existingEnrollments = await _enrollmentRepository.GetEnrollmentsForParticipantOnDate(  eventId, participantId, eventDate);

            // Если есть пересечения, значит, участник не может посетить событие.
            return !existingEnrollments.Any();
        }
        public async Task<EventDTO> GetEventById(int id)
        {
            var eventItem = await _eventRepository.GetEventById(id); // Получаем Event из репозитория
            if (eventItem == null)
            {
                return null;
            }

            // Получаем изображение из кэша или из базы данных
            eventItem.Image = await GetEventImage(id, eventItem.Image);

            // Получаем количество зарегистрированных участников

            EventStatus status = await _eventRepository.CalculateEventStatus( eventItem);
            EventDTO eventDto = _mapper.Map<EventDTO>(eventItem);
            eventDto.Status = status;

            return eventDto;
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
        



    }
}
