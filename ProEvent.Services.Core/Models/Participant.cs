
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvent.Services.Core.Models
{
 
        public class Participant
        {
            public int Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DateTime DateOfBirth { get; set; }


            public string Email { get; set; }

            public string UserId { get; set; } // Foreign key to AspNetUsers

            public ApplicationUser User { get; set; } // Навигационное свойство

            public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        }
    }

