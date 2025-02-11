using AutoMapper;
using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<EventDTO, Event>();
            CreateMap<Event, EventDTO>();

            CreateMap<EnrollmentDTO, Enrollment>();
            CreateMap<Enrollment, EnrollmentDTO>();

            CreateMap<ParticipantDTO, Participant>();
            CreateMap<Participant, ParticipantDTO>();
        }
    }
}
