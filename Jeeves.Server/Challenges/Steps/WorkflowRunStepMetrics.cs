using System;

namespace Jeeves.Server.Challenges.Steps
{
    [Flags]
    public enum WorkflowRunStepMetrics
    {
        None = 0,
        Cpu  = 1 << 0,
        Ram  = 1 << 1,
        Time = 1 << 2,
        All = Cpu | Ram | Time
    }
}