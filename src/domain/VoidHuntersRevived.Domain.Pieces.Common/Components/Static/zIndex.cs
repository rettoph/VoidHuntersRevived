using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Static
{
    public struct zIndex : IEntityComponent
    {
        public readonly float Value;

        public zIndex(float value)
        {
            Value = value;
        }
    }
}
