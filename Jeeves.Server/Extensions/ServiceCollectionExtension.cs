using System.Linq;
using Jeeves.Server.Challenges.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Jeeves.Server.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddWorkflowStepResolver(this IServiceCollection services)
        {
            var stepContract = typeof(IWorkflowStep);
            var stepTypes = stepContract.Assembly.GetTypes().Where(t => stepContract.IsAssignableFrom(t)).ToArray();

            services.AddSingleton<IWorkflowStepResolver, WorkflowStepResolver>(provider => new WorkflowStepResolver(stepTypes));
        }
    }
}