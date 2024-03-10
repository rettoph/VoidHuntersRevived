using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Static
{
    public struct zIndex : IEntityComponent
    {
        public readonly int Value;

        public zIndex(int value)
        {
            Value = value;
        }
    }
}
