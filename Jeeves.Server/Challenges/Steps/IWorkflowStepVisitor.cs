namespace Jeeves.Server.Challenges.Steps
{
    public interface IWorkflowStepVisitor
    {
        void Visit(WorkflowRunStep step);
        void Visit(WorkflowAssertStep step);
    }
}