using System;
using System.Collections.Generic;
using System.IO;
using Jeeves.Server.Challenges.Steps;
using Jeeves.Server.Extensions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Jeeves.Server.Challenges.Converters
{
    public class WorkflowStepConverter : IYamlTypeConverter
    {
        private readonly IWorkflowStepResolver _stepResolver;
        
        public WorkflowStepConverter(IWorkflowStepResolver stepResolver)
        {
            _stepResolver = stepResolver;
        }
        
        public bool Accepts(Type type)
        {
            return type == typeof(IWorkflowStep);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var (properties, step) = GetStepProperties(parser);
            var stepType = _stepResolver.Resolve(properties);
            var deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance)
                                                        .WithTypeConverter(new WorkflowRunStepMetricsConverter())
                                                        .Build();
            return deserializer.Deserialize(step, stepType);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            throw new NotImplementedException();
        }
        
        private (List<string> properties, string step) GetStepProperties(IParser parser)
        {
            using var output = new StringWriter();
            var emitter = new Emitter(output);
            var properties = new List<string>();
            var expectingPropertyName = true;
            
            emitter.Emit(new StreamStart());
            emitter.Emit(new DocumentStart());
            emitter.Emit(parser.Current!);
            
            if (!parser.MoveNext())
                throw new Exception("Failed to parse workflow. Unexpected end of workflow");
            
            while (parser.Current.IsNot<MappingEnd>())
            {
                emitter.Emit(parser.Current!);

                switch (parser.Current)
                {
                    case MappingStart _:
                        ProcessUntil<MappingEnd>(parser, emitter);
                        expectingPropertyName = true;
                        break;
                    case SequenceStart _:
                        ProcessUntil<SequenceEnd>(parser, emitter);
                        expectingPropertyName = true;
                        break;
                    case Scalar scalar when expectingPropertyName:
                        properties.Add(PascalCaseNamingConvention.Instance.Apply(scalar.Value));
                        expectingPropertyName = false;
                        break;
                    case Scalar _:
                        expectingPropertyName = true;
                        break;
                    default:
                        throw new Exception($"Failed to parse workflow. Unexpected token at {parser.Current.Start}:{parser.Current.End}");
                }

                if (!parser.MoveNext())
                    throw new Exception("Failed to parse workflow. Unexpected end of workflow");
            }
            emitter.Emit(parser.Current!);
            emitter.Emit(new DocumentEnd(true));
            emitter.Emit(new StreamEnd());
            parser.MoveNext();
            
            return (properties, output.ToString());
        }

        private void ProcessUntil<T>(IParser parser, IEmitter emitter) where T : ParsingEvent
        {
            while (parser.Current.IsNot<T>())
            {
                if (!parser.MoveNext())
                    throw new Exception("Failed to parse workflow. Unexpected end of workflow");
                
                emitter.Emit(parser.Current!);
            }
        }
    }
}