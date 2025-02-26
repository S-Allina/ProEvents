using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.BLL.DTOs;
using ProEvent.BLL.Interfaces.IService;
using ProEvent.DAL.Interfaces.IRepository;

namespace ProEvent.WebApp.Controllers
{
    [ApiController]
    [Route("/events")]
    public class eventAPIController : Controller
    {
        protected ResponseDTO _response;
        private readonly ILogger<eventAPIController> _logger;
        private IEventService _eventService;

        public eventAPIController(ResponseDTO response, ILogger<eventAPIController> logger, IEventService eventService)
        {
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
            string? name = null,
            bool isPassed = false,
            CancellationToken cancellationToken = default)

        {
            var (events, totalCount) = await _eventService.GetEvents(
                            pageNumber,
                            pageSize,
                            startDate,
                            endDate,
                            location,
                            category,
                            name,
                            isPassed,
                            cancellationToken);

            var response = new
            {
                Events = events,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(response);

        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<object> GetEventsByUser(string userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<EventWithRegistrationDTO> eventDTOs = await _eventService.GetEventByUserId(userId, cancellationToken);
            _response.Result = eventDTOs;

            return _response;
        }

        [HttpGet("{id}")]
        public async Task<object> GetById(int id, CancellationToken cancellationToken = default)
        {
            EventDTO eventDTOs = await _eventService.GetEventById(id, cancellationToken);
            _response.Result = eventDTOs;

            return _response;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Post([FromBody] EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            EventDTO createdEvent = await _eventService.CreateUpdateEvent(eventDTO, cancellationToken);
            _response.Result = createdEvent;
            _response.DisplayMessage = "Событие успешно создано.";
            return CreatedAtAction(nameof(GetEvents), new { id = createdEvent.Id }, _response);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put([FromBody] EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            EventDTO updatedEvent = await _eventService.CreateUpdateEvent(eventDTO, cancellationToken);
            _response.Result = updatedEvent;
            _response.DisplayMessage = "Обновление прошло успешно.";
            return Ok(_response);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            bool isSuccess = await _eventService.DeleteEvent(id, cancellationToken);
            _response.IsSuccess = isSuccess;
            _response.Result = true;
            _response.DisplayMessage = "Удаление прошло успешно";
            return Ok(_response);
        }
    }
}