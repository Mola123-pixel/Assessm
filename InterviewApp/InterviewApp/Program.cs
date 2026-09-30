using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MediatR;

namespace InterviewApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Models.GreetingOptions>(context.Configuration.GetSection("Greeting"));

                    // Core services
                    services.AddTransient<Services.IGreetingService, Services.GreetingService>();
                    services.AddTransient<Services.ITimeGreetingService, Services.TimeGreetingService>();

                    // Register MediatR and ensure handlers are registered.
                    services.AddMediatR(typeof(Program).Assembly);

                    // Explicit registration for the GetTimeGreeting handler in case assembly scanning misses it
                    services.AddTransient<MediatR.IRequestHandler<Services.GetTimeGreetingQuery, string>, Services.GetTimeGreetingHandler>();
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                var mediator = host.Services.GetRequiredService<IMediator>();
                var greeting = await mediator.Send(new Requests.GreetUserCommand());
                Console.WriteLine(greeting);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Application terminated due to an error.");
                // Exit gracefully
                return;
            }

            await host.RunAsync();
        }
    }
}