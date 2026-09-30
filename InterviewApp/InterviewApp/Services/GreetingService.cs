using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MediatR;
using InterviewApp.Models;

namespace InterviewApp.Services
{
    public class GreetingService : IGreetingService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GreetingService> _logger;

        public GreetingService(IMediator mediator, ILogger<GreetingService> logger)
        {
            _mediator = mediator;
            _logger = logger;

            _logger.LogInformation("GreetingService starting.");
        }

        public async Task<string> GetGreetingAsync()
        {
            // Get time-based greeting via MediatR
            string timeGreeting = await _mediator.Send(new GetTimeGreetingQuery());

            var fullMessage = $"Hello! {timeGreeting}";
            _logger.LogInformation("Composed greeting: {Greeting}", fullMessage);
            return fullMessage;
        }
    }
}