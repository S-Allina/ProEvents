using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Repository;
using ProEvent.Services.Core.Validators;
using ProEvent.Services.Infrastructure.Repository;
using ProEvents.Service.Core.DTOs;

namespace ProEvent.WebApp.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("/participants")]
    public class ParticipantAPIController : Controller
    {
        protected ResponseDTO _response;
        private IParticipantRepository _participantRepository;
        private readonly IValidator<Participant> _participantValidator;

        public ParticipantAPIController(IParticipantRepository participantRepository, ResponseDTO response, IValidator<Participant> participantValidator)
        {
            _participantRepository = participantRepository;
            this._response = new ResponseDTO();
            _participantValidator = participantValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var participants = await _participantRepository.GetParticipants();

            // Create a response object that includes the events and pagination information
            var response = new
            {
                participants = participants
            };

            return Ok(response);
        }
       
        [HttpGet]
        //[Authorize]
        [Route("GetByUserId/{id}")]
        public async Task<object> GetByUserId(string id)
        {
            try
            {
                ParticipantDTO participantDTO = await _participantRepository.GetParticipantByUserId(id);
                _response.Result = participantDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
       //метод создания вызывается при регистрации
        [HttpPut]
        public async Task<object> Put([FromBody] ParticipantDTO participantDTO)
        {
            try
            {
                ParticipantDTO model = await _participantRepository.CreateUpdateParticipant(participantDTO);
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
                bool isSuccess = await _participantRepository.DeleteParticipant(id);
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