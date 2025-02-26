using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvent.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Participant Participant { get; set; }
    }

}
