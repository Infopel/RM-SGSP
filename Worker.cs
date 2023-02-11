using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGSP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public static string ApiCommand { get; set; } = "no command";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}, command: {string}", DateTimeOffset.Now, ApiCommand);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
