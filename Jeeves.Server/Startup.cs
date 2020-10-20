using AutoMapper;
using Jeeves.Server.Repositories;
using Jeeves.Server.Services;
using Jeeves.Server.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jeeves.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(GetType().Assembly);
            services.AddSingleton<IUsersService, UsersService>();
            services.AddSingleton<IUsersRepository, UsersRepository>();
            services.AddSingleton<IChallengesService, ChallengesService>();
            services.AddSingleton<IChallengesRepository, ChallengesRepository>();
            services.AddSingleton<IAttemptsService, AttemptsService>();
            services.AddSingleton<IAttemptsRepository, AttemptsRepository>();
            
            services.AddSingleton<IWorkerCreator, LocalWorkerCreator>();
            services.AddSingleton<WorkersForeman>();
            services.AddHostedService(sp => sp.GetRequiredService<WorkersForeman>());
            services.AddSingleton<IWorkersForeman>(provider => provider.GetRequiredService<WorkersForeman>());
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}