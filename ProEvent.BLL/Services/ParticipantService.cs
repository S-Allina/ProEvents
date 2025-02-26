

using AutoMapper;
using FluentValidation;
using ProEvent.BLL.DTOs;
using ProEvent.BLL.Interfaces.IService;
using ProEvent.DAL.Interfaces.IRepository;
using ProEvent.DAL.Repository;
using ProEvent.Domain.Models;

namespace ProEvent.BLL.Services
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
            var participants = await _participantRepository.GetParticipants(cancellationToken);
            return _mapper.Map<List<ParticipantDTO>>(participants);
        }

        public async Task<ParticipantDTO> GetParticipantByUserId(string userId, CancellationToken cancellationToken)
        {
            var participant = await _participantRepository.GetParticipantByUserId(userId, cancellationToken);
            return _mapper.Map<ParticipantDTO>(participant);
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
            var resultParticipant = await _participantRepository.CreateUpdateParticipant(participant, cancellationToken);

            return _mapper.Map<ParticipantDTO>(resultParticipant);
        }

        public async Task<bool> DeleteParticipant(int id, CancellationToken cancellationToken)
        {
            return await _participantRepository.DeleteParticipant(id, cancellationToken);
        }
    }
}
