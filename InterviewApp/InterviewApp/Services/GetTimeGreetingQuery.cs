using MediatR;

namespace InterviewApp.Services
{
    public record GetTimeGreetingQuery() : IRequest<string>;
}