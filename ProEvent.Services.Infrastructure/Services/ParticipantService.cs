using AutoMapper;
using FluentValidation;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Interfaces.IRepository;
using ProEvent.Services.Core.Interfaces.IService;
using ProEvent.Services.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Infrastructure.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IValidator<Participant> _participantValidator;
        private readonly IMapper _mapper;

        public ParticipantService(IParticipantRepository participantRepository, IValidator<Participant> participantValidator, IMapper mapper)
        {
            _participantRepository = participantRepository;
            _participantValidator = participantValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipantDTO>> GetParticipants(CancellationToken cancellationToken)
        {
            return await _participantRepository.GetParticipants(cancellationToken);
        }

        public async Task<ParticipantDTO> GetParticipantByUserId(string userId, CancellationToken cancellationToken)
        {
            return await _participantRepository.GetParticipantByUserId(userId, cancellationToken);
        }

        public async Task<ParticipantDTO> CreateUpdateParticipant(ParticipantDTO participantDTO, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Participant participant = _mapper.Map<ParticipantDTO, Participant>(participantDTO);

            var validationResult = await _participantValidator.ValidateAsync(participant, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                throw new ValidationException(string.Join(Environment.NewLine, errors));
            }

            return await _participantRepository.CreateUpdateParticipant(participantDTO, cancellationToken);
        }

        public async Task<bool> DeleteParticipant(int id, CancellationToken cancellationToken)
        {
            return await _participantRepository.DeleteParticipant(id, cancellationToken);
        }
    }
}
