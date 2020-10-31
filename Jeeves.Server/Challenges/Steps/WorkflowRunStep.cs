using YamlDotNet.Serialization;

namespace Jeeves.Server.Challenges.Steps
{
    public class WorkflowRunStep : IWorkflowStep
    {
        public string Name { get; set; }
        
        [YamlMember(Alias = "run")]
        public string Command { get; set; }
        
        [YamlMember(Alias = "with")]
        public WorkflowRunStepOptions Options { get; set; }

        public void Accept(IWorkflowStepVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}