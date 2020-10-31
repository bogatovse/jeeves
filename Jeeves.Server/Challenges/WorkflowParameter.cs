namespace Jeeves.Server.Challenges
{
    public class WorkflowParameter
    {
        public string Name { get; set; } = string.Empty;
        public bool Required { get; set; } = false;
        public string Default { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}