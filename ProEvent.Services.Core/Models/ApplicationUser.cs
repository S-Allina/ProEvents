using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvent.Services.Core.Models
{
        public class ApplicationUser : IdentityUser
        {
            [ForeignKey("UserId")]
            public Participant Participant { get; set; }
        }
    
}
