using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.BLL.DTOs;
using ProEvent.BLL.Interfaces.IService;

namespace ProEnrollment.WebApp.Controllers
{
    [ApiController]
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
            _response.Result = enrollment;
            return Ok(_response);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Post([FromBody] EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken = default)
        {
            EnrollmentDTO model = await _enrollmentService.CreateUpdateEnrollment(enrollmentDTO, cancellationToken);
            _response.Result = model;
            _response.DisplayMessage = "Вы успешно зарегистрированы!";
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = model.Id }, _response);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put([FromBody] EnrollmentDTO enrollmentDTO, CancellationToken cancellationToken = default)
        {
            EnrollmentDTO model = await _enrollmentService.CreateUpdateEnrollment(enrollmentDTO, cancellationToken);
            _response.Result = model;
            _response.DisplayMessage = "Запись обновлена успешно";
            return Ok(_response);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            bool isSuccess = await _enrollmentService.DeleteEnrollment(id, cancellationToken);
            _response.IsSuccess= isSuccess;
            _response.Result = true;
            _response.DisplayMessage = "Запись удалена успешно";
            return Ok(_response);
        }
    }
}