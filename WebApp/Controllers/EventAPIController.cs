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
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 4)
        {
            var (events, totalCount) = await _eventRepository.GetEvents(pageNumber, pageSize);

            // Create a response object that includes the events and pagination information
            var response = new
            {
                Events = events,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(response);
        }
        [HttpGet]
        [Route("GetEventsByUser/{userId}")]
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

        [HttpGet("filtered")]
        public async Task<object> GetFilteredEvents(DateTime? startDate, DateTime? endDate, string? location, string? category)
        {
            try
            {
                IEnumerable<EventDTO> eventDTOs = await _eventRepository.GetFilteredEvents(startDate, endDate, location, category);
                _response.Result = eventDTOs;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("search")]
        public async Task<object> GetEventsByName(string name)
        {
            try
            {
                IEnumerable<EventDTO> eventDTOs = await _eventRepository.GetEventByName(name);
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
            try
            {
                EventDTO model = await _eventService.CreateUpdateEvent(eventDTO);
                _response.Result = model;
                _response.DisplayMessage = "Операция прошла успешно.";
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (ProEvent.Services.Core.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Введены не корректные данные.");
                _response.IsSuccess = false;
                _response.DisplayMessage = "Ошибка валидации.";
                _response.ErrorMessages = ex.Errors;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating/updating event.");
                _response.IsSuccess = false;
                _response.DisplayMessage = "Error creating/updating event.";
                _response.ErrorMessages = new List<string?> { ex.Message };
                return StatusCode(500, _response);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EventDTO eventDTO)
        {
            try
            {
                EventDTO model = await _eventService.CreateUpdateEvent(eventDTO);
                _response.Result = model;
                _response.DisplayMessage = "Обновление прошло успешно.";
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (ProEvent.Services.Core.Exceptions.ValidationException ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Введены некорректные данные.";
                _response.ErrorMessages = ex.Errors;
                return BadRequest(_response);
            }
            catch (ArgumentException ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Событие не найдено";
                _response.ErrorMessages = new List<string?> { ex.Message };
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Ошибка обновления события";
                _response.ErrorMessages = new List<string?> { ex.Message };
                return StatusCode(500, _response);
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool isSuccess = await _eventService.DeleteEvent(id);
                if (isSuccess)
                {
                    _response.Result = true;
                    _response.DisplayMessage = "Удаление прошло успешно";
                    _response.IsSuccess = true;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.DisplayMessage = "Событие не найдено.";
                    _response.ErrorMessages = new List<string?> { "Событие не найдено." };
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Не удалось удалить событие.";
                _response.ErrorMessages = new List<string?> { ex.Message };
                return StatusCode(500, _response);
            }
        }
    }
}