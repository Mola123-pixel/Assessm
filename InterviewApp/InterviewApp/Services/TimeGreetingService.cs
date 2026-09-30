using System;
using System.Threading.Tasks;

namespace InterviewApp.Services
{
    public class TimeGreetingService : ITimeGreetingService
    {
        public Task<string> GetTimeGreetingAsync()
        {
            return Task.FromResult(GetTimeGreeting());
        }

        public string GetTimeGreeting()
        {
            var now = DateTime.Now;
            if (now.Hour < 12) return "Good morning";
            if (now.Hour < 18) return "Good afternoon";
            return "Good evening";
        }
    }
}