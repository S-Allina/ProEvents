using ProEvent.Services.Core.DTOs;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Interfaces
{
    public interface IEventService
    {
        Task<bool> CanParticipantAttendEvent(int participantId, DateTime eventDate, int eventId);
        Task<EventDTO> GetEventById(int id);
        Task<EventDTO> CreateUpdateEvent(EventDTO eventDTO);
        Task<bool> DeleteEvent(int id);


        //Task<bool> IsParticipantEnrolled(int eventId, int participantId);
    }
}
