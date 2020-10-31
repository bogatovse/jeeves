using YamlDotNet.Core.Events;

namespace Jeeves.Server.Extensions
{
    public static class ObjectExtension
    {
        public static bool IsNot<T>(this ParsingEvent parsingEvent) where T : ParsingEvent
        {
            return !(parsingEvent is T);
        }
    }
}