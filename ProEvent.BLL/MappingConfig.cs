using AutoMapper;
using ProEvent.BLL.DTOs;
using ProEvent.Domain.Models;

namespace ProEvent.BLL
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
