using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGSP.Controllers.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers.Api
{
    [ApiController]
    [Route("api/[Controller]/{apiCommand}")]
    public class CommandController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private string RunningMessage() => $"apiCommand:{Worker.ApiCommand}";

        public CommandController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string SetCommand(string apiCommand)
        {
            Worker.ApiCommand = apiCommand;
            _logger.LogInformation(RunningMessage());
            return RunningMessage();
        }
    }
}
