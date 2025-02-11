
namespace ProEvent.Services.Core.DTOs
{
 
        public class ParticipantDTO
        {
            public int Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DateTime DateOfBirth { get; set; }


            public string Email { get; set; }

            public string UserId { get; set; } // Foreign key to AspNetUsers


        }
    }

