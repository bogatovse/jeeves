namespace Jeeves.Server.Challenges.Steps
{
    public interface IWorkflowStep
    {
        string Name { get; }
        void Accept(IWorkflowStepVisitor visitor);
    }
}