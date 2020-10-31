using Jeeves.Server.Challenges.Converters;
using Jeeves.Server.Challenges.Steps;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Jeeves.Server.Challenges
{
    public class WorkflowParser
    {
        private readonly IWorkflowStepResolver _stepResolver;
        
        public WorkflowParser(IWorkflowStepResolver stepResolver)
        {
            _stepResolver = stepResolver;
        }
        
        public Workflow Parse(string workflow)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .WithTypeConverter(new WorkflowParametersConverter())
                .WithTypeConverter(new WorkflowStepConverter(_stepResolver))
                .Build();

            return deserializer.Deserialize<Workflow>(workflow);
        }
    }
}