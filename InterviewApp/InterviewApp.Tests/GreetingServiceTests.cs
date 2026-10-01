using System;
using System.Threading;
using System.Threading.Tasks;
using InterviewApp.Models;
using InterviewApp.Services;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace InterviewApp.Tests;

public class GreetingServiceTests
{
    [Fact]
    public async Task GetGreetingAsync_UsesConfiguredLanguageGreeting_WhenLanguageIsSupported()
    {
        var service = new GreetingService(
            new StubMediator("Good afternoon"),
            Options.Create(new GreetingOptions
            {
                Message = "Welcome to the interview app!",
                Language = "Zulu"
            }),
            NullLogger<GreetingService>.Instance);

        var greeting = await service.GetGreetingAsync();

        Assert.Contains("Welcome to the interview app!", greeting);
        Assert.Contains("Good afternoon", greeting);
        Assert.Contains("Sawubona", greeting);
    }

    [Fact]
    public async Task GetGreetingAsync_FallsBackToEnglish_WhenLanguageIsUnsupported()
    {
        var service = new GreetingService(
            new StubMediator("Good evening"),
            Options.Create(new GreetingOptions
            {
                Message = "Welcome to the interview app!",
                Language = "French"
            }),
            NullLogger<GreetingService>.Instance);

        var greeting = await service.GetGreetingAsync();

        Assert.Contains("Hello", greeting);
        Assert.Contains("Good evening", greeting);
    }

    [Fact]
    public void Constructor_Throws_WhenMessageIsEmpty()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new GreetingService(
            new StubMediator("Good morning"),
            Options.Create(new GreetingOptions
            {
                Message = string.Empty,
                Language = "English"
            }),
            NullLogger<GreetingService>.Instance));

        Assert.Contains("Greeting message cannot be null or empty", exception.Message);
    }

    private sealed class StubMediator : IMediator
    {
        private readonly string _timeGreeting;

        public StubMediator(string timeGreeting)
        {
            _timeGreeting = timeGreeting;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is GetTimeGreetingQuery)
            {
                return Task.FromResult((TResponse)(object)_timeGreeting);
            }

            throw new NotSupportedException($"Unexpected request type: {request.GetType().Name}");
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Task.CompletedTask;
        }
    }
}
