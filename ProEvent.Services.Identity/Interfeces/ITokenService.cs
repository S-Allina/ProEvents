using ProEvent.Services.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Identity.Interfeces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user); // или ClaimsPrincipal, в зависимости от вашей реализации
                                                    // Дополнительные методы для валидации токена, если необходимо
    }
}
