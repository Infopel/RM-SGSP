using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface IEmailRepository
    {
        Task SendEMailAsync(EmailRequest emailRequest);
    }
}
