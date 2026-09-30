using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InterviewApp.Services;

namespace InterviewApp.Requests
{
    public record GreetUserCommand() : IRequest<string>;

    public class GreetUserCommandHandler : IRequestHandler<GreetUserCommand, string>
    {
        private readonly IGreetingService _greetingService;

        public GreetUserCommandHandler(IGreetingService greetingService)
        {
            _greetingService = greetingService;
        }

        public async Task<string> Handle(GreetUserCommand request, CancellationToken cancellationToken)
        {
            return await _greetingService.GetGreetingAsync();
        }
    }
}