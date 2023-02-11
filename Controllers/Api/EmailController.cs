using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGSP.Dtos;
using SGSP.Models;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        public EmailController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromForm] EmailRequest request)
        {
            try
            {
                await _emailRepository.SendEMailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {

                throw new Exception("My Custom Error Message", ex);
            }
        }
    }
}
