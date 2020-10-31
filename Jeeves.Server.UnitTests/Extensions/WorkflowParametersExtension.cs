using System.Collections.Generic;
using System.Text;
using Jeeves.Server.Challenges;

namespace Jeeves.Server.UnitTests.Extensions
{
    public static class WorkflowParametersExtension
    {
        public static string ToYaml(this Dictionary<string, WorkflowParameter> parameters)
        {
            var yaml = new StringBuilder();
            yaml.AppendLine("parameters: ");

            foreach (var parameter in parameters)
            {
                yaml.AppendLine($"  {parameter.Key}:");
                yaml.AppendLine($"    name: {parameter.Value.Name}");
                yaml.AppendLine($"    description: {parameter.Value.Description}");
                yaml.AppendLine($"    default: {parameter.Value.Default}");
                yaml.AppendLine($"    required: {parameter.Value.Required}");
            }

            return yaml.ToString();
        }
    }
}