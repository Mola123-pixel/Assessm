using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InterviewApp.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InterviewApp.Services
{
    public class GreetingService : IGreetingService
    {
        private const string DefaultLanguage = "English";
        private static readonly IReadOnlyDictionary<string, string> SupportedLanguageGreetings =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["English"] = "Hello",
                ["Afrikaans"] = "Hallo",
                ["Zulu"] = "Sawubona"
            };

        private readonly IMediator _mediator;
        private readonly ILogger<GreetingService> _logger;
        private readonly GreetingOptions _options;

        public GreetingService(IMediator mediator, IOptions<GreetingOptions> options, ILogger<GreetingService> logger)
        {
            _mediator = mediator;
            _logger = logger;
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("GreetingService starting for language '{Language}'.", _options.Language);
            ValidateConfiguration();
        }

        public async Task<string> GetGreetingAsync()
        {
            try
            {
                ValidateConfiguration();

                string timeGreeting = await _mediator.Send(new GetTimeGreetingQuery());
                string configuredMessage = _options.Message.Trim();
                string language = _options.Language.Trim();
                string localizedGreeting = GetLocalizedGreeting(language);

                var fullMessage = $"{configuredMessage} {timeGreeting}. {localizedGreeting}!";
                _logger.LogInformation("Composed greeting: {Greeting}", fullMessage);
                return fullMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the greeting.");
                throw;
            }
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_options.Message))
            {
                _logger.LogError("Greeting configuration is invalid. Message is null or empty.");
                throw new InvalidOperationException("Greeting message cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(_options.Language))
            {
                _logger.LogError("Greeting configuration is invalid. Language is null or empty.");
                throw new InvalidOperationException("Greeting language cannot be null or empty.");
            }
        }

        private string GetLocalizedGreeting(string language)
        {
            if (SupportedLanguageGreetings.TryGetValue(language, out string greeting))
            {
                return greeting;
            }

            _logger.LogWarning(
                "Unsupported language '{Language}' supplied. Falling back to '{FallbackLanguage}'.",
                language,
                DefaultLanguage);

            return SupportedLanguageGreetings[DefaultLanguage];
        }
    }
}