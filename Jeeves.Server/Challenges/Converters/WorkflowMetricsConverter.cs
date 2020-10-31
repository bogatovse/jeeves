using System;
using Jeeves.Server.Challenges.Steps;
using Jeeves.Server.Extensions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Jeeves.Server.Challenges.Converters
{
    public class WorkflowRunStepMetricsConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(WorkflowRunStepMetrics);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            if (parser.Current is SequenceStart)
            {
                return ParseSequence(parser);
            }
            else
            {
                if (!(parser.Current is Scalar scalar))
                    throw new Exception("Unable to parse metrics");
                
                var metrics =  Enum.Parse<WorkflowRunStepMetrics>(scalar.Value, true);
                
                if (!parser.MoveNext())
                    throw new Exception("Failed to parse workflow. Unexpected end of workflow");

                return metrics;
            }
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            throw new NotImplementedException();
        }

        private WorkflowRunStepMetrics ParseSequence(IParser parser)
        {
            WorkflowRunStepMetrics metrics = WorkflowRunStepMetrics.None;
            
            while (parser.Current.IsNot<SequenceEnd>())
            {
                if (parser.Current is Scalar metric)
                {
                    metrics |= Enum.Parse<WorkflowRunStepMetrics>(metric.Value, true);
                }
                
                if (!parser.MoveNext())
                    throw new Exception("Failed to parse workflow. Unexpected end of workflow");
            }
            
            if (!parser.MoveNext())
                throw new Exception("Failed to parse workflow. Unexpected end of workflow");

            return metrics;
        }
    }
}