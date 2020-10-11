using System;
using Bogus;
using Jeeves.Server.Domain;

namespace Jeeves.Server.IntegrationTests.Fakes
{
    public static class Fakers
    {
        public static Faker<User> User { get; }
        
        static Fakers()
        {
            User = new Faker<User>()
                .StrictMode(true)
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName());
        }
    }
}