using Svelto.ECS;

namespace VoidHuntersRevived.Common.Pieces.Components.Static
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
