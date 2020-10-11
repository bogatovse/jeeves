using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Jeeves.Server.IntegrationTests.Factories
{
    public class JeevesWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly Action<IServiceCollection> _registrations;
        
        public JeevesWebApplicationFactory() : this(null) { }

        public JeevesWebApplicationFactory(Action<IServiceCollection> registrations)
        {
            _registrations = registrations;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                _registrations?.Invoke(services);
            });
        }
    }
}