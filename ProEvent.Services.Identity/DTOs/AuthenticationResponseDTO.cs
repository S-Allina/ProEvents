using ProEvents.Service.Core.DTOs;


namespace ProEvent.Services.Identity.DTOs
{
    public class AuthenticationResponseDTO : ResponseDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
