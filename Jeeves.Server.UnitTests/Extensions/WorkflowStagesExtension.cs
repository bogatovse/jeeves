using System.Collections.Generic;
using System.Text;
using Jeeves.Server.Challenges;

namespace Jeeves.Server.UnitTests.Extensions
{
    public static class WorkflowStagesExtension
    {
        public static string ToYaml(this Dictionary<string, WorkflowStage> stages)
        {
            var yaml = new StringBuilder();
            yaml.AppendLine("stages: ");

            foreach (var stage in stages)
            {
                yaml.AppendLine($"  {stage.Key}:");
                yaml.AppendLine($"    steps:");

                if (stage.Value.Steps != null)
                {
                    foreach (var step in stage.Value.Steps)
                    {
                        yaml.AppendLine($"      name: {step.Name}");
                    }
                }
            }
            
            return yaml.ToString();
        }
    }
}