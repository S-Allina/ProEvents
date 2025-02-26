using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.BLL.DTOs;
using ProEvent.BLL.Interfaces.IService;
using System.Security.Claims;

namespace ProEvent.WebApp.Controllers
{
    [ApiController]
    [Route("/participants")]
    public class ParticipantAPIController : Controller
    {
        protected ResponseDTO _response;
        private readonly IParticipantService _participantService;

        public ParticipantAPIController(IParticipantService participantService, ResponseDTO response)
        {
            _participantService = participantService;
            this._response = new ResponseDTO();
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var participants = await _participantService.GetParticipants(cancellationToken);

            var response = new
            {
                participants = participants
            };

            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetByUserId/{id}")]
        public async Task<object> GetByUserId(string id, CancellationToken cancellationToken = default)
        {
                ParticipantDTO participantDTO = await _participantService.GetParticipantByUserId(id, cancellationToken);
                _response.Result = participantDTO;
           
            return _response;
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<object> Put([FromBody] ParticipantDTO participantDTO, CancellationToken cancellationToken = default)
        {
            
                ParticipantDTO model = await _participantService.CreateUpdateParticipant(participantDTO, cancellationToken);
                _response.Result = model;
            return _response;
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<object> Delete(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSuccess = await _participantService.DeleteParticipant(id, cancellationToken);
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