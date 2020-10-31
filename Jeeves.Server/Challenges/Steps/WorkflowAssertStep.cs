namespace Jeeves.Server.Challenges.Steps
{
    public class WorkflowAssertStep : IWorkflowStep
    {
        public string Name { get; set; }
        public string Assert { get; set; }
        
        public void Accept(IWorkflowStepVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}