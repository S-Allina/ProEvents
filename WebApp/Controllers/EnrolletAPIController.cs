using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces.IService;
using ProEvent.Services.Infrastructure.Repository;
using ProEvent.Services.Infrastructure.Services;
using ProEvents.Service.Core.DTOs;

namespace ProEnrollment.WebApp.Controllers
{
    [Route("/enrollments")]
    public class EnrollmentAPIController : Controller
    {
        protected ResponseDTO _response;
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentAPIController(IEnrollmentService enrollmentService)
        {
            this._response = new ResponseDTO();
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments(int? eventId, CancellationToken cancellationToken = default)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsWithParticipantInfo(eventId, cancellationToken);
            _response.Result = enrollments;
            return Ok(_response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id, CancellationToken cancellationToken = default)
        {
            EnrollmentDTO enrollment = await _enrollmentService.GetEnrollmentById(id, cancellationToken);

            if (enrollment == null)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Запись не найдена";
                return NotFound(_response);
            }

            _response.Result = enrollment;
            return Ok(_response);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken = default)
        {
            EnrollmentDTO model = await _enrollmentService.CreateUpdateEnrollment(enrollmentDTO, cancellationToken);
            _response.Result = model;
            _response.DisplayMessage = "Вы успешно зарегистрированы!";
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = model.Id }, _response);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken = default)
        {
            EnrollmentDTO model = await _enrollmentService.CreateUpdateEnrollment(enrollmentDTO, cancellationToken);
            _response.Result = model;
            _response.DisplayMessage = "Запись обновлена успешно";
            return Ok(_response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            bool isSuccess = await _enrollmentService.DeleteEnrollment(id, cancellationToken);

            if (isSuccess)
            {
                _response.Result = true;
                _response.DisplayMessage = "Запись удалена успешно";
                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Запись не найдена.";
                return NotFound(_response);
            }
        }
    }
}