using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using YamlDotNet.Serialization;
using PascalCaseNamingConvention = YamlDotNet.Serialization.NamingConventions.PascalCaseNamingConvention;

namespace Jeeves.Server.Challenges.Steps
{
    public interface IWorkflowStepResolver
    {
        Type Resolve(IReadOnlyCollection<string> yamlProperties);
    }
    
    public class WorkflowStepResolver : IWorkflowStepResolver
    {
        private readonly Type[] _stepTypes;
        
        public WorkflowStepResolver(Type[] stepTypes)
        {
            _stepTypes = stepTypes;
        }
        
        public Type Resolve(IReadOnlyCollection<string> yamlProperties)
        {
            foreach (var stepType in _stepTypes)
            {
                var stepProperties = stepType.GetProperties().Select(GetStepPropertyName).ToHashSet();
                if (stepProperties.IsSupersetOf(yamlProperties))
                {
                    return stepType;
                }
            }
            
            throw new Exception($"Unable to resolve step based on provided properties ({string.Join(",", yamlProperties)})");
        }

        private string GetStepPropertyName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<YamlMemberAttribute>();
            return PascalCaseNamingConvention.Instance.Apply(attribute?.Alias ?? property.Name);
        }
    }
}