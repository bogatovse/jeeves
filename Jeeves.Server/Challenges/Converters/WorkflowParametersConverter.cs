using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Jeeves.Server.Challenges.Converters
{
    public class WorkflowParametersConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Dictionary<string, WorkflowParameter>);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            
            switch (parser.Current)
            {
                case SequenceStart _:
                {
                    var parametersNames = deserializer.Deserialize<string[]>(parser);
                    return parametersNames?.Select(p => new WorkflowParameter {Name = p}).ToDictionary(p => p.Name, p => p);
                }
                case Scalar _:
                {
                    var parameter = deserializer.Deserialize<string>(parser);
                    if (string.IsNullOrEmpty(parameter))
                    {
                        return null;
                    }
                    else
                    {
                        return new Dictionary<string, WorkflowParameter>
                        {
                            { parameter, new WorkflowParameter { Name = parameter } }
                        };
                    }
                }
                case MappingStart _:
                {
                    var parameters = deserializer.Deserialize<Dictionary<string, WorkflowParameter>>(parser);
                    foreach (var parameter in parameters.Where(parameter => string.IsNullOrEmpty(parameter.Value.Name)))
                    {
                        parameter.Value.Name = parameter.Key;
                    }
                    return parameters;
                }
                default:
                    throw new ArgumentOutOfRangeException(parser.Current?.ToString());
            }
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}