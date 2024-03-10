using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface ISimpleDebugEngine : IEngine
    {
        public class SimpleDebugLine
        {
            public readonly string Group;
            public readonly string Title;
            public readonly Func<string> Value;

            public SimpleDebugLine(string group, string title, Func<string> value)
            {
                Group = group;
                Title = title;
                Value = value;
            }
        }

        SimpleDebugLine[] Lines { get; }
    }
}
