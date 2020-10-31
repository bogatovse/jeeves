using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.Serialization;

namespace Jeeves.Server.Challenges
{
    public class Workflow
    {
        public string Name { get; set; } = string.Empty;
        [YamlMember(Alias = "runs-on")]
        public string HostType { get; set; }
        public Dictionary<string, WorkflowParameter> Parameters { get; set; }
        public Dictionary<string, WorkflowStage> Stages { get; set; }
    }
}