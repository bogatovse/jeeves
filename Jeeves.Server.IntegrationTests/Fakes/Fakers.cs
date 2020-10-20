using System;
using Bogus;
using Jeeves.Server.Domain;

namespace Jeeves.Server.IntegrationTests.Fakes
{
    public static class Fakers
    {
        public static Faker<User> User { get; }
        public static Faker<Challenge> Challenge { get; }
        
        public static Faker<ChallengeAttempt> Attempt { get; }
        
        static Fakers()
        {
            User = new Faker<User>()
                .StrictMode(true)
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName());
            
            Challenge = new Faker<Challenge>()
                .StrictMode(true)
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, f => f.Lorem.Word());
            
            Attempt = new Faker<ChallengeAttempt>()
                .StrictMode(true)
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Status, f => f.Random.Enum<ChallengeAttemptStatus>())
                .RuleFor(c => c.Solution, f => f.Random.Bytes(1024));
            
        }
    }
}