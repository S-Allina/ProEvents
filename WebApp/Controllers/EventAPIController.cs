using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
using ProEvent.Services.Core.Validators;
using ProEvents.Service.Core.DTOs;

namespace ProEvent.WebApp.Controllers
{
    [ApiController]
    [Route("/events")]
    public class eventAPIController : Controller
    {
        protected ResponseDTO _response;
        private IEventRepository _eventRepository;
        private readonly ILogger<eventAPIController> _logger;
        private IEventService _eventService;

        public eventAPIController(IEventRepository eventRepository, ResponseDTO response, ILogger<eventAPIController> logger, IEventService eventService)
        {
            _eventRepository = eventRepository;
            this._response = new ResponseDTO();
            _logger = logger;
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(
     int pageNumber = 1,
     int pageSize = 4,
     DateTime? startDate = null,
     DateTime? endDate = null,
     string? location = null,
     string? category = null,
     string? name = null)
        {
            try
            {
                var (events, totalCount) = await _eventService.GetEvents(
                    pageNumber,
                    pageSize,
                    startDate,
                    endDate,
                    location,
                    category,
                    name);

                var response = new
                {
                    Events = events,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("getByUserId/{userId}")]
        public async Task<object> GetEventsByUser(string userId)
        {
            try
            {
                IEnumerable<EventWithRegistrationDTO> eventDTOs = await _eventRepository.GetEventsByUser(userId);
                _response.Result = eventDTOs;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<object> GetById(int id)
        {
            try
            {
                EventDTO eventDTOs = await _eventService.GetEventById(id);
                _response.Result = eventDTOs;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventDTO eventDTO)
        {
            EventDTO createdEvent = await _eventService.CreateUpdateEvent(eventDTO);

            _response.Result = createdEvent;
            _response.DisplayMessage = "Событие успешно создано.";
            return CreatedAtAction(nameof(GetEvents), new { id = createdEvent.Id }, _response);
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EventDTO eventDTO)
        {
            EventDTO updatedEvent = await _eventService.CreateUpdateEvent(eventDTO);
            _response.Result = updatedEvent;
            _response.DisplayMessage = "Обновление прошло успешно.";
            return Ok(_response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isSuccess = await _eventService.DeleteEvent(id);

            if (isSuccess)
            {
                _response.Result = true;
                _response.DisplayMessage = "Удаление прошло успешно";
                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Событие не найдено.";
                return NotFound(_response);
            }
        }
    }
}