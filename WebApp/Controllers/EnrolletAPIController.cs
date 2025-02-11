using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Repository;
using ProEvents.Service.Core.DTOs;

namespace ProEnrollment.WebApp.Controllers
{
    //[Authorize]
    [Route("/enrollment")]
    public class EnrollmentAPIController : Controller
    {
        protected ResponseDTO _response;
        private IEnrollmentRepository _enrollmentRepository;
        private readonly IEventService _eventService;

        public EnrollmentAPIController(IEnrollmentRepository enrollmentRepository,IEventService eventService)
        {
            _enrollmentRepository = enrollmentRepository;
            _eventService = eventService;
            this._response = new ResponseDTO();
        }
      
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<EnrollmentDTO> enrollmentDTOs = await _enrollmentRepository.GetEnrollments();
                     _response.Result = enrollmentDTOs;
            }
            catch (Exception ex)
            {
_response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpGet]
        [Route("GetByEventId/{id}")]
        public async Task<object> GetByEventId(int id)
        {
            try
            {
                // Получаем Enrollments для указанного EventId
                var enrollments = await _enrollmentRepository.GetEnrollmentsByEventId(id); // Предполагается, что такой метод есть в вашем репозитории

                // Преобразуем Enrollments в ParticipantWithRegistrationDTO
                var participantWithRegistrationDTOs = enrollments.Select(enrollment => new ParticipantWithRegistrationDTO
                {
                    ParticipanId = enrollment.Participant.Id,
                    EnrollmentId= enrollment.Id,
                    FirstName = enrollment.Participant.FirstName,
                    LastName = enrollment.Participant.LastName,
                    DateOfBirth = enrollment.Participant.DateOfBirth,
                    Email = enrollment.Participant.Email,
                    UserId = enrollment.Participant.UserId,
                    RegistrationDate = enrollment.RegistrationDate // Получаем RegistrationDate из Enrollment
                }).ToList();

                _response.Result = participantWithRegistrationDTOs;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet]
        //[Authorize]
        [Route("{id}")]
        public async Task<object> GetById(int id)
        {
            try
            {
                EnrollmentDTO enrollmentDTOs = await _enrollmentRepository.GetEnrollmentById(id);
                _response.Result = enrollmentDTOs;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }



        [HttpPost]
        public async Task<object> Post([FromBody] EnrollmentDTO enrollmentDTO)
        {
            try
            {
                var eventItem = await _eventService.GetEventById(enrollmentDTO.EventId); //Тут я надеюсь ты используешь EventService для получения EventDTO
                if (eventItem == null)
                {
                    _response.IsSuccess = false;
                    _response.DisplayMessage = "Событие не найдено";
                    return _response;
                }
                DateTime eventDate = eventItem.Date;

                enrollmentDTO.ParticipantId = await _enrollmentRepository.GetParticipantIdByUserId(enrollmentDTO.UserId);

                if (enrollmentDTO.ParticipantId == null)
                {
                    _response.IsSuccess = false;
                    _response.DisplayMessage = "Участник не найден";
                    return _response;
                }

                bool canAttend = await _eventService.CanParticipantAttendEvent((int)enrollmentDTO.ParticipantId, eventDate, enrollmentDTO.EventId);

                if (!canAttend)
                {
                    _response.IsSuccess = false;
                    _response.DisplayMessage = "Ошибка, вы на это время уже зарегистрированы на другое событие";
                    return _response;
                }

                EnrollmentDTO model = await _enrollmentRepository.CreateUpdateEnrollment(enrollmentDTO);
                _response.Result = model;
                _response.DisplayMessage = "Вы успешно зарегистрированы!"; // Сообщение об успехе

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Произошла ошибка при регистрации.";
                _response.ErrorMessages = new List<string> { ex.Message }; // Сообщение об ошибке
            }
            return _response;
        }
        [HttpPut]
        public async Task<object> Put([FromBody] EnrollmentDTO enrollmentDTO)
        {
            try
            {
                EnrollmentDTO model = await _enrollmentRepository.CreateUpdateEnrollment(enrollmentDTO);
                _response.Result = model;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete]
        //[Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<object> Delete(int id)
        {
            try
            {
                bool isSuccess = await _enrollmentRepository.DeleteEnrollment(id);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
    }
}